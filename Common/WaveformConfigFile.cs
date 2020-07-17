using System.IO;

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
    }
    public class TdmsFile : WaveformConfigFile
    {
        internal TdmsFile(string filePath)
            : base(filePath) { }
    }
    public class RfwsFile : WaveformConfigFile
    {
        internal RfwsFile(string filePath)
            : base(filePath) { }
    }
}
