using System.IO;
using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.Utilities.WaveformParsing
{
    public abstract class WaveformConfigFile
    {
        public string FilePath { get; }

        protected WaveformConfigFile(string filePath) 
        {
            FilePath = filePath;
        }

        public static WaveformConfigFile Load(string filePath)
        {
            string ext = Path.GetExtension(filePath);
            switch (ext)
            {
                case ".tdms":
                    return new TdmsFile(filePath);
                case ".rfws":
                    return new RfwsFile(filePath);
                default:
                    return null;
            }
        }

        public abstract void SaveConfiguration(RFmxInstrMX instr);
        public abstract void SaveConfiguration(RFmxInstrMX instr, string outputDirectory);
    }
    public class TdmsFile : WaveformConfigFile
    {
        internal TdmsFile(string filePath)
            : base(filePath) { }

        public override void SaveConfiguration(RFmxInstrMX instr)
        {
            throw new System.NotImplementedException();
        }

        public override void SaveConfiguration(RFmxInstrMX instr, string outputDirectory)
        {
            throw new System.NotImplementedException();
        }
    }
    public class RfwsFile : WaveformConfigFile
    {
        internal RfwsFile(string filePath)
            : base(filePath) { }

        public override void SaveConfiguration(RFmxInstrMX instr)
        {
            string directory = Path.GetDirectoryName(FilePath);
            SaveConfiguration(instr, directory);
        }

        public override void SaveConfiguration(RFmxInstrMX instr, string outputDirectory)
        {
            string newFileName = "RFmx_Config_" + Path.GetFileNameWithoutExtension(FilePath) + ".tdms";

            string newPath = Path.Combine(outputDirectory, newFileName);

            instr.SaveAllConfigurations(newPath);
        }
    }
}
