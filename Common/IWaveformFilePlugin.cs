using System;
using NationalInstruments.RFmx.InstrMX;
using Serilog;

namespace NationalInstruments.Utilities.WaveformParsing
{
    public interface IWaveformFilePlugin
    {
        string[] SupportedRFmxVersions { get; }

        bool CanParse(WaveformConfigFile file);
        void Parse(WaveformConfigFile file, RFmxInstrMX instr);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class WaveformFilePlugInAttribute : Attribute
    {
        public string Description;
        public float[] RFmxVersions;
    }
}
