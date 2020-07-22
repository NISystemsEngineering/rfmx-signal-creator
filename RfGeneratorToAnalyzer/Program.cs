using System;
using NationalInstruments.RFmx.InstrMX;
using System.IO;
using CommandLine;
using CommandLine.Text;
using System.Linq;
using CommandLine.Text;
using System.Collections.Generic;
using Serilog;
using Serilog.Events;
using Serilog.Context;
using NationalInstruments.Utilities.WaveformParsing;

namespace NationalzInstruments.Utilities.WaveformParsing
{
    class Program
    {
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
                };
            static void Main(string[] args)
            {
                Parser.Default.ParseArguments<Options>(args).WithParsed(o => Execute(o));
                /*
                var parser = new Parser(settings => settings.HelpWriter = null);
                var parserResult = parser.ParseArguments<Options>(args);
                var helpText = HelpText.AutoBuild(parserResult, h =>
                {
                    //configure HelpText
                    h.AddPreOptionsLine("This is a test!!"); //remove newline between options
                    h.Heading = "Myapp 2.0.0-beta"; //change header
                    h.Copyright = "Copyright (c) 2019 Global.com"; //change copyright text
                                                                   // more options ...
                    return h;
                }, e => e);

                parserResult.WithParsed(o => Execute(o));
                parserResult.WithNotParsed(errs => Console.WriteLine(helpText));*/
            }
        }
        public static void Execute(Options o)
        {
            #region Logger Configuration
            string logOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{Properties:j}{NewLine}{Exception}";

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
                   //.Filter.ByIncludingOnly( e => e.Level == LogEventLevel.Information || e.Level == LogEventLevel.Fatal)
                .CreateLogger();

            Log.Verbose("--------------------------------------------------------");
            Log.Verbose("Beginning new execution");
            #endregion
            try
            {
                WaveformPluginFactory.LoadPlugins();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Plugin load failed; see log for more details. Exiting...");
                return;
            }

            IEnumerable<string> filesToParse = new List<string>();

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
                var waveform = WaveformConfigFile.Load(file);
                string fileName = Path.GetFileName(file);

                IWaveformFilePlugin matchedPlugin;
                try
                {
                    matchedPlugin = WaveformPluginFactory.LoadedPlugins.Where(p => p.CanParse(waveform)).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error in plugin(s) evaluating whether file \"{File}\" can be parsed; skiping", fileName);
                    break;
                }

                if (matchedPlugin != null)
                {
                    Log.Information("Processing file {File} using {Plugin} supporting RFmx version(s) {Versions}",
                        fileName, matchedPlugin.GetType().Name, matchedPlugin.SupportedRFmxVersions);

                    RFmxInstrMX instr = new RFmxInstrMX(fileName, "AnalysisOnly=1");
                    matchedPlugin.Parse(waveform, instr);

                    if (string.IsNullOrEmpty(o.OutputDirectory))
                        waveform.SaveConfiguration(instr);
                    else
                        waveform.SaveConfiguration(instr, o.OutputDirectory);

                    instr.Dispose();
                }
                else
                {
                    Log.Debug("Skipping {file}; no installed plugin is compatible with this file.", fileName);
                }
            }
            Log.CloseAndFlush();
        }

    }
}
