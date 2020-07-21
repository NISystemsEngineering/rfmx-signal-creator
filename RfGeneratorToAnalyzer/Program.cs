using System;
using NationalInstruments.RFmx.InstrMX;
using System.IO;
using CommandLine;
using System.Linq;
using CommandLine.Text;
using System.Collections.Generic;

namespace NationalInstruments.Utilities.WaveformParsing.Example
{
    class Program
    {
        public class Options
        {
            [Value(0, MetaName = "test", HelpText = "Specifies a single file to parse or a folder of waveform files to parse", Required = true)]
            public string Path { get; set; }
            [Option('o', "outputdir", HelpText = "Alternate directry to output configuration files to; default is in the same directory.")]
            public string OutputDirectory { get; set; }
        }
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o => Execute(o));
            Console.ReadKey();
        }
        public static void Execute(Options o)
        {
            WaveformPluginFactory.LoadPlugins();
            
            IEnumerable<string> filesToParse = new List<string>();

            if (File.Exists(o.Path))
            {
                filesToParse = new string[1] { o.Path };
            }
            else if (Directory.Exists(o.Path))
            {
                filesToParse = Directory.EnumerateFiles(o.Path, "*.tdms")
                    .Union(Directory.EnumerateFiles(o.Path, "*.rfws"));
            }

            foreach (string file in filesToParse)
            {
                var waveform = WaveformConfigFile.Load(file);
                string fileName = Path.GetFileName(file);
                var matchedPlugin = WaveformPluginFactory.LoadedPlugins.Where(p => p.CanParse(waveform)).FirstOrDefault();

                if (matchedPlugin != null)
                {
                    Console.WriteLine($"Processing file \"{fileName}\" with plugin {matchedPlugin.GetType()}");

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
                    Console.WriteLine($"No suitable plugin found for parsing file \"{fileName}\"");
                }
            }
        }
    }
}