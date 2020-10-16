using System;
using NationalInstruments.RFmx.InstrMX;
using System.IO;
using CommandLine;
using CommandLine.Text;
using System.Linq;
using System.Collections.Generic;
using Serilog;
using Serilog.Events;

namespace NationalInstruments.Utilities.SignalCreator
{
    class Program
    {
        #region Command Line Options Configuraiton
        public class Options
        {
            [Value(0, MetaName = "<Paths>", HelpText = "Specifies one or more paths to load; paths can be a single waveform configuration " +
                "file or a directory of configuration files", Required = true)]
            public IEnumerable<string> Path { get; set; }
            [Option('o', "outputdir", HelpText = "Alternate directory to output configuration files to; default is in the same directory.")]
            public string OutputDirectory { get; set; }
            [Option('v', "verbose", HelpText = "Enable verbose logging in the log file and optionally the console if -c is set.")]
            public bool Verbose { get; set; }
            [Option('c', "console", HelpText = "Sends full file log to console in addition to the log file.")]
            public bool LogToConsole { get; set; }

            [Usage(ApplicationAlias = "RFmxSignalCreator")]
            public static IEnumerable<Example> Examples =>
                new List<Example>()
                {
                    new Example("Process a single waveform configuration", new Options { Path = new string[] { @"C:\waveform.rfws" } }),
                    new Example("Process a directory containing multiple waveform configurations",
                        new UnParserSettings {PreferShortName = true },
                        new Options { Path = new string[] { @"C:\Waveform Configurations" }, OutputDirectory = @"C:\RFmx Configurations" }),
                    new Example("Process multiple files and diretories containing multiple waveform configurations",
                        new UnParserSettings {PreferShortName = true },
                        new Options { Path = new string[] { "waveform1.rfws", "waveform2.rfws", @"Waveforms\MoreFiles" },
                            OutputDirectory = @"C:\RFmx Configurations" }),
                    new Example("Process a directory with verbose logging to the console",
                        new UnParserSettings {PreferShortName = true },
                        new Options { Path = new string[] { @"C:\Waveform Configurations" }, LogToConsole = true, Verbose = true })
                };
        }
        #endregion
        static void Main(string[] args)
        {
            // Parse command line arguments and hand them over to Execute
            var result = Parser.Default.ParseArguments<Options>(args);
            result.WithParsed(o => Execute(o));
            result.WithNotParsed(o =>
            {
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            });
        }
        public static void Execute(Options o)
        {
            #region Logger Configuration
            string logOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}" +
                                       "                    {Properties:j}{NewLine}{Exception}";
            string consoleOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

            LogEventLevel fileLogLevel = LogEventLevel.Debug;
            if (o.Verbose)
            {
                fileLogLevel = LogEventLevel.Verbose;
            }

            LogEventLevel consoleLogLevel = LogEventLevel.Information;
            if (o.LogToConsole)
            {
                consoleLogLevel = fileLogLevel;
                consoleOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}" +
                                        "               {Properties: j}{NewLine}{Exception}";
            }

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .WriteTo.File("Log.txt", outputTemplate: logOutputTemplate, restrictedToMinimumLevel: fileLogLevel)
                .WriteTo.Logger(lc => lc
                    .WriteTo.Console(consoleLogLevel, outputTemplate: consoleOutputTemplate))
                .CreateLogger();

            Log.Debug("--------------------------------------------------------");
            #endregion

            Log.Information("Initializing...");

            #region Plugin Loading
            // Load plugins first before loading any files
            try
            {
                WaveformPluginFactory.LoadPlugins();
            }
            catch (DllNotFoundException)
            {
                Log.Fatal("No assemblies were found in the local plugins directory. Ensure that one or more valid plugin are " +
                    "located in the plugins directory ({FullPluginDirectory})", WaveformPluginFactory.FullPluginDirectoryPath);
                return;
            }
            catch (MissingMemberException)
            {
                Log.Fatal("No supported plugins were found in the local plugin directory, or none could be loaded successfully. " +
                    "Ensure that one or more valid plugin are located in the plugins directory ({FullPluginDirectory})",
                    WaveformPluginFactory.FullPluginDirectoryPath);
                return;
            }
            catch (Exception ex)
            {
                // Any other exception is still likely a fatal exception
                Log.Fatal(ex, "Unhandled exception occured during plugin load; see log for more details. Exiting...");
                return;
            }
            #endregion

            #region File Parsing
            IEnumerable<string> filesToParse = Enumerable.Empty<string>();
            List<string> individualFiles = new List<string>();

            foreach (string path in o.Path)
            {
                // Determine if we have a file or folder path specified from the command line
                if (File.Exists(path))
                {
                    individualFiles.Add(path);
                }
                else if (Directory.Exists(path))
                {
                    Log.Verbose("Loading files from directory {Path}", path);
                    filesToParse = filesToParse.Concat(Directory.EnumerateFiles(path, "*.tdms"))
                        .Concat(Directory.EnumerateFiles(path, "*.rfws"));
                }
                else
                {
                    Log.Error("File or directory \"{Path}\" does not exist", path);
                }
            }
            filesToParse = individualFiles.Concat(filesToParse);

            foreach (string file in filesToParse)
            {
                var waveform = WaveformConfigFileType.Load(file);
                string fileName = Path.GetFileName(file);

                // Check all loaded plugins to see if any can parse this file
                IWaveformFilePlugin matchedPlugin;
                try
                {
                    Log.Verbose("Checking for compatible plugins for {File}", fileName);
                    matchedPlugin = WaveformPluginFactory.LoadedPlugins.Where(p => p.CanParse(waveform)).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error in plugin(s) evaluating whether file \"{File}\" can be parsed; skiping", fileName);
                    continue;
                }

                if (matchedPlugin != null)
                {
                    Log.Information("Processing file {File} using {Plugin}", fileName, matchedPlugin.GetType().Name);

                    using (RFmxInstrMX instr = new RFmxInstrMX(fileName, "AnalysisOnly=1"))
                    {
                        try
                        {
                            matchedPlugin.Parse(waveform, instr);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Unhandled parsing exception plugin in {Plugin}", matchedPlugin.GetType().Name);
                            continue;
                        }

                        try
                        {
                            if (string.IsNullOrEmpty(o.OutputDirectory))
                                waveform.SaveConfiguration(instr);
                            else
                                waveform.SaveConfiguration(instr, o.OutputDirectory);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Error saving configuration file");
                        }
                    }

                }
                else
                {
                    Log.Warning("Skipping {File}; no installed plugin is compatible with this file.", fileName);
                }
            }
            Log.Information("Execution complete");
            Log.Debug("--------------------------------------------------------");
            Log.CloseAndFlush();
        }
        #endregion
    }
}
