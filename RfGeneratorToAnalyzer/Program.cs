using System;
using NationalInstruments.RFmx.InstrMX;
using System.IO;
using CommandLine;
using CommandLine.Text;
using System.Linq;
using System.Collections.Generic;
using Serilog;
using Serilog.Events;
using NationalInstruments.Utilities.WaveformParsing;

namespace NationalInstruments.Utilities.WaveformParsing
{
    class Program
    {
        #region Command Line Options Configuraiton
        public class Options
        {
            [Value(0, MetaName = "<Path>", MetaValue = @"C:\pathToFile", HelpText = "Specifies a single file to parse or a folder of waveform files to parse", Required = true)]
            public string Path { get; set; }
            [Option('o', "outputdir", HelpText = "Alternate directry to output configuration files to; default is in the same directory.")]
            public string OutputDirectory { get; set; }
            [Option('v', "verbose", HelpText = "Enable verbose logging in the log file and optionally the console if -c is set.")]
            public bool Verbose { get; set; }
            [Option('c', "console", HelpText = "Sends full file log to console in addition to the log file.")]
            public bool LogToConsole { get; set; }

            [Usage(ApplicationAlias = "app")]
            public static IEnumerable<Example> Examples =>
                new List<Example>()
                {
                    new Example("Process a single waveform configuration", new Options { Path = @"C:\waveform.rfws" }),
                    new Example("Process a directory containing multiple waveform configurations", 
                        new Options { Path = @"C:\waveform.rfws", OutputDirectory = @"C:\Exported Configurations" }),
                            CenterFrequency = 28.0e9 }),
                    new Example("Process a directory with verbose logging to the console",
                        new UnParserSettings {PreferShortName = true, GroupSwitches = true },
                        new Options {OutputDirectory = @"C:\Exported Configurations", LogToConsole = true, Verbose = true })
                };
        }
        #endregion
            static void Main(string[] args)
            {
            // Parse command line arguments and hand them over to Execute
                Parser.Default.ParseArguments<Options>(args).WithParsed(o => Execute(o));
        }
        public static void Execute(Options o)
        {
            #region Logger Configuration
            string logOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}" +
                                       "                    {Properties:j}{NewLine}{Exception}";

            LogEventLevel fileLogLevel = LogEventLevel.Debug;
            if (o.Verbose)
            {
                fileLogLevel = LogEventLevel.Verbose;
            }

            LogEventLevel consoleLogLevel = LogEventLevel.Information;
            if (o.LogToConsole)
            {
                consoleLogLevel = fileLogLevel;
            }

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .WriteTo.File("Log.txt", outputTemplate: logOutputTemplate, restrictedToMinimumLevel: fileLogLevel)
                .WriteTo.Logger(lc => lc
                    .WriteTo.Console(consoleLogLevel))
                .CreateLogger();

            Log.Debug("--------------------------------------------------------");
            Log.Debug("Beginning new execution");
            #endregion

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
            IEnumerable<string> filesToParse = new List<string>();

            // Determine if we have a file or folder path specified from the command line
            if (File.Exists(o.Path))
            {
                filesToParse = new string[1] { o.Path };
            }
            else if (Directory.Exists(o.Path))
            {
                filesToParse = Directory.EnumerateFiles(o.Path, "*.tdms")
                    .Concat(Directory.EnumerateFiles(o.Path, "*.rfws"));
            }

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
                    break;
                }

                if (matchedPlugin != null)
                {
                    Log.Information("Processing file {File} using {Plugin}", fileName, matchedPlugin.GetType().Name);

                    RFmxInstrMX instr = new RFmxInstrMX(fileName, "AnalysisOnly=1");

                    try
                    {
                    matchedPlugin.Parse(waveform, instr);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Unhandled parsing exception plugin in {Plugin}", matchedPlugin.GetType().Name);
                        break;
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

                    instr.Dispose();
                }
                else
                {
                    Log.Warning("Skipping {file}; no installed plugin is compatible with this file.", fileName);
                }
            }
            Log.Debug("Execution complete");
            Log.Debug("--------------------------------------------------------");
            Log.CloseAndFlush();
        }
        #endregion
    }
}
