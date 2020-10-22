using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NationalInstruments.RFmx.InstrMX;
using Serilog;
using Serilog.Events;
using CommandLine;

namespace NationalInstruments.Utilities.SignalCreator
{
    using Plugins;
    class Program
    {
        static void Main(string[] args)
        {
            // Parse command line arguments and hand them over to Execute
            var result = Parser.Default.ParseArguments<CommandLineOptions>(args);
            result.WithParsed(o => Execute(o));
            result.WithNotParsed(o =>
            {
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            });
        }
        public static void Execute(CommandLineOptions o)
        {
            ConfigureLogger(o);
            Log.Debug("--------------------------------------------------------");
            Log.Information("Initializing...");

            // Load plugins first before loading any files
            try
            {
                WaveformPluginFactory.LoadPlugins();
            }
            catch (DllNotFoundException)
            {
                Log.Fatal("No assemblies (.dll) were found in the local plugins directory. Ensure that one or more valid plugin assemblies are " +
                    "located in the plugins directory ({FullPluginDirectory})", WaveformPluginFactory.FullPluginDirectoryPath);
                return;
            }
            catch (MissingMemberException)
            {
                Log.Fatal("No supported plugins were found in the local plugin directory among the assemblies, or none could be loaded successfully. " +
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

            IEnumerable<string> filesToParse = EnumerateWaveformFiles(o);
            ParseFiles(filesToParse, o);

            Log.Information("Execution complete");
            Log.Debug("--------------------------------------------------------");
            Log.CloseAndFlush();
        }
        /// <summary>
        /// Configures the logger that will be used throughout the program to log information, warnings, and errors.
        /// </summary>
        /// <param name="o">Specifies the command line options when the application is invoked.</param>
        private static void ConfigureLogger(CommandLineOptions o)
        {
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
        }
        /// <summary>
        /// Enumerates all supported waveform files based on the configuration in <paramref name="o"/>.<para></para>
        /// 
        /// If one or more waveform files is specified, they will be added directly to enumeration. Any directories specified
        /// will be searched for valid waveform files and added to the enumeration.
        /// </summary>
        /// <param name="o">Specifies the command line options when the application is invoked.</param>
        /// <returns></returns>
        private static IEnumerable<string> EnumerateWaveformFiles(CommandLineOptions o)
        {
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
            return filesToParse;
        }
        /// <summary>
        /// Checks each waveform file in <paramref name="filesToParse"/> to see if any loaded plugin is compatible with the file.
        /// If a compatible plugin is found, then the file will be parsed with the identified plugin. If parsing is successful,
        /// the RFmx configuration file will be saved according to the settings in <paramref name="o"/>.
        /// </summary>
        /// <param name="filesToParse">Specifies the files to be parsed if a valid plugin is present.</param>
        /// <param name="o">Specifies the command line options when the application is invoked.</param>
        private static void ParseFiles(IEnumerable<string> filesToParse, CommandLineOptions o)
        {
            foreach (string file in filesToParse)
            {
                var waveform = WaveformConfigFileType.Load(file);
                string fileName = Path.GetFileName(file);

                // Check all loaded plugins to see if any can parse this file
                IWaveformFilePlugin matchedPlugin;
                try
                {
                    Log.Information("Checking for compatible plugins for {File}", fileName);
                    matchedPlugin = WaveformPluginFactory.LoadedPlugins.Where(p => p.CanParse(waveform)).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error in plugin(s) evaluating whether file \"{File}\" can be parsed; skiping", fileName);
                    continue;
                }

                if (matchedPlugin != null)
                {
                    using (RFmxInstrMX instr = new RFmxInstrMX(fileName, "AnalysisOnly=1"))
                    {
                        Log.Information("Processing file {File} using {Plugin}", fileName, matchedPlugin.GetType().Name);
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
                    Log.Warning("Skipping {File}; no installed plugin is compatible with this file", fileName);
                }
            }
        }
    }
}
