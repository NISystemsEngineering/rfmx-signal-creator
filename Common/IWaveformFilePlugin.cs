using System;
using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.Utilities.WaveformParsing
{
    public interface IWaveformFilePlugin
    {
        bool CanParse(WaveformConfigFile file);
        void Parse(WaveformConfigFile file, RFmxInstrMX instr);
    }
}
