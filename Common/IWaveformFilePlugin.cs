using System;
using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.Utilities.WaveformParsing
{
    public interface IWaveformFilePlugin
    {
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
