using System;
using System.IO;
using NationalInstruments.RFmx.InstrMX;
using Serilog;

namespace NationalInstruments.Utilities.SignalCreator
{
    /// <summary>
    /// Represents a waveform configuration file type.
    /// </summary>
    public abstract class WaveformConfigFileType
    {
        /// <summary>Specifies the path to the waveform configuration file.</summary>
        public string FilePath { get; }
        public string FileName => Path.GetFileName(FilePath);
        /// <summary>Specifies the prefix that that will be prepended to the output TDMS file.</summary>
        public string RFmxFilePrefix { get; set; } = "RFmx_Config_";

        protected WaveformConfigFileType(string filePath) 
        {
            FilePath = filePath;
        }

        /// <summary>
        /// A factory method to select the appropriate concrete class instance based on the file extension.
        /// </summary>
        /// <exception cref="FormatException">Thrown when the file format does not match any specific implementations.</exception>
        /// <param name="filePath">Specifies the file path to load.</param>
        /// <returns>A specific <see cref="WaveformConfigFileType"/> instance matching the type.</returns>
        public static WaveformConfigFileType Load(string filePath)
        {
            string ext = Path.GetExtension(filePath);
            switch (ext)
            {
                case ".tdms":
                    return new TdmsFile(filePath);
                case ".rfws":
                    return new RfwsFile(filePath);
                default:
                    throw new FormatException("File is not a TDMS or RFWS file");
            }
        }

        /// <summary>Saves the RFmx Instr configuration file as is appropriate to the file type.</summary>
        /// <param name="instr">The RFmx Instr configuration to save.</param>
        public void SaveConfiguration(RFmxInstrMX instr)
        {
            string directory = Path.GetDirectoryName(FilePath);
            SaveConfiguration(instr, directory);
        }
        /// <summary>Saves the RFmx Instr configuration file as is appropriate to the file type.</summary>
        /// <param name="instr">Specifies the RFmx Instr configuration to save.</param>
        /// <param name="outputDirectory">Specifies the output directory in which to save the file.</param>
        public virtual void SaveConfiguration(RFmxInstrMX instr, string outputDirectory)
        {
            string newFileName = RFmxFilePrefix + Path.GetFileNameWithoutExtension(FilePath) + ".tdms";

            string absoluteDirectoryPath = Path.GetFullPath(outputDirectory);
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(absoluteDirectoryPath);
                Log.Debug("Created directory {Path}", absoluteDirectoryPath);
            }

            string newPath = Path.Combine(absoluteDirectoryPath, newFileName);
            Log.Debug("Output path {Path}", newPath);

            instr.SaveAllConfigurations(newPath);
            Log.Information("Configuration saved to {Path}", newPath);
        }
    }
    /// <summary>
    /// Represents a TDMS waveform configuration file created from the NI RFmx Waveform Creator for WLAN or Bluetooth.
    /// </summary>
    public class TdmsFile : WaveformConfigFileType
    {
        internal TdmsFile(string filePath)
            : base(filePath) { }
    }
    /// <summary>
    /// Represents an RFWS waveform configuration file created from the NI RFmx Waveform Creator.
    /// </summary>
    public class RfwsFile : WaveformConfigFileType
    {
        internal RfwsFile(string filePath)
            : base(filePath) { }

        public override void SaveConfiguration(RFmxInstrMX instr, string outputDirectory)
        {
            // NOTE 
            // Ideal behavior for this plugin would be to first invoke RFmx WC to compile the RFWS file into a TDMS file,
            // save the RFmx configuration into another file, then merge the two files creating a single file with the 
            // waveform and RFmx configuration. However, as of RFmx 20.0 having additional TDMS channels in the RFmx
            // configuration results in an error, so for now the TDMS file is saved independently.

            base.SaveConfiguration(instr, outputDirectory);
        }
    }
}
