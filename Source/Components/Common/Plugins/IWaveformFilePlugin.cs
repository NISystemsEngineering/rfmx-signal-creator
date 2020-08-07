using System;
using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.Utilities.WaveformParsing
{
    /// <summary>
    /// Represents a waveform configuring file parsing pluging. Any plugin must also implement the <see cref="WaveformFilePlugInAttribute"></see>.
    /// </summary>
    public interface IWaveformFilePlugin
    {
        /// <summary>
        /// Determines whether this plugin can parse the waveform configuration file specfied by <paramref name="file"/>.
        /// </summary>
        /// <param name="file">Specifies the file to examine.</param>
        bool CanParse(WaveformConfigFileType file);
        /// <summary>
        /// Parses the waveform configuration <paramref name="file"/> and applies the properties to the RFmx session <paramref name="instr"/>.
        /// </summary>
        /// <param name="file">Specifies the waveform configuration to parse.</param>
        /// <param name="instr">Specifes the RFmx Instr session to apply the properties to.</param>
        void Parse(WaveformConfigFileType file, RFmxInstrMX instr);
    }

    /// <summary>
    /// Represents additional information about a plugin. This attribute must be implemented along with <see cref="IWaveformFilePlugin"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class WaveformFilePlugInAttribute : Attribute
    {
        /// <summary>Specfies the plugin descriptions.</summary>
        public string Description { get; }
        /// <summary>Specifies which RFmx version(s) the plugin supports.</summary>
        public string[] RFmxVersions { get; }

        /// <param name="description">Specfies the plugin descriptions.</param>
        /// <param name="supportedRfmxVersions">Specifies which RFmx version(s) the plugin supports.</param>
        public WaveformFilePlugInAttribute(string description, params string[] supportedRfmxVersions)
        {
            Description = description;
            RFmxVersions = supportedRfmxVersions;
        }
    }
}
