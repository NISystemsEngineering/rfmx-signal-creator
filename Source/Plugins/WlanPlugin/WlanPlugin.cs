using System;
using NationalInstruments.RFmx.InstrMX;
using System.IO;
using NationalInstruments.RFToolkits.Interop;
using Serilog;
using Serilog.Context;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.WlanPlugin
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

                WlanSignalGroup signal = wlan.Deserialize<WlanSignalGroup>();
                instr.CreateWlanSignalConfigurationFromObject(signal);
            }
        }
    }
}
