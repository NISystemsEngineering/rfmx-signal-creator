using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.WlanMX;
using Serilog;
using Serilog.Context;
using System.IO;
using NationalInstruments.RFToolkits.Interop;

namespace NationalInstruments.Utilities.SignalCreator.Plugins
{
    [WaveformFilePlugIn("Parses TDMS waveform files containing configurations from the RFmx Waveform Creator for WLAN", "20.0")]
    public class WlanPlugin : IWaveformFilePlugin
    {
        public niWLANG wlan;
        public bool CanParse(WaveformConfigFileType file)
        {
            using (LogContext.PushProperty("Plugin", nameof(WlanPlugin)))
            {
                if (file is TdmsFile)
                {
                    wlan = new niWLANG(niWLANGConstants.CompatibilityVersion050000);
                    try
                    {
                        wlan.LoadConfigurationFromFile(file.FilePath, niWLANGConstants.True);
                    }
                    catch (Exception ex)
                    {
                        Log.Verbose(ex, "CanParse return false because an exception occured loading the file");
                        return false;
                    }
                    return true;
                }
                else
                {
                    Log.Verbose("CanParse returning false because file is not a TDMS file");
                    return false;
                }
            }
        }

        public void Parse(WaveformConfigFileType file, RFmxInstrMX instr)
        {
            using (LogContext.PushProperty("Plugin", nameof(WlanPlugin)))
            {
                if (wlan == null)
                {
                    bool result = CanParse(file);
                    if (!result) throw new InvalidOperationException($"{file.FileName} is not a valid file for this plugin.");
                }
                WlanParser parser = new WlanParser(wlan);

                WlanSignal signal = new WlanSignal("");
                parser.Parse(signal);

                var rfmxWlanSingal = instr.GetWlanSignalConfiguration();

                WlanRFmxMapper mapper = new WlanRFmxMapper(rfmxWlanSingal);
                mapper.Map(signal);
            }
        }
    }
}
