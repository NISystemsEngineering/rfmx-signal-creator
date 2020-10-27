using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace NationalInstruments.Utilities.SignalCreator
{
    /// <summary>
    /// Defines the command line options for the application.
    /// </summary>
    public class CommandLineOptions
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

        [Usage(ApplicationAlias = "SignalCreator")]
        public static IEnumerable<Example> Examples =>
            new List<Example>()
            {
                    new Example("Process a single waveform configuration", new CommandLineOptions { Path = new string[] { @"C:\waveform.rfws" } }),
                    new Example("Process a directory containing multiple waveform configurations",
                        new UnParserSettings {PreferShortName = true },
                        new CommandLineOptions { Path = new string[] { @"C:\Waveform Configurations" }, OutputDirectory = @"C:\RFmx Configurations" }),
                    new Example("Process multiple files and diretories containing multiple waveform configurations",
                        new UnParserSettings {PreferShortName = true },
                        new CommandLineOptions { Path = new string[] { "waveform1.rfws", "waveform2.rfws", @"Waveforms\MoreFiles" },
                            OutputDirectory = @"C:\RFmx Configurations" }),
                    new Example("Process a directory with verbose logging to the console",
                        new UnParserSettings {PreferShortName = true },
                        new CommandLineOptions { Path = new string[] { @"C:\Waveform Configurations" }, LogToConsole = true, Verbose = true })
            };
    }
}
