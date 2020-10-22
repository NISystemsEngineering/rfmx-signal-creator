using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.NRMX;
using Serilog;
using Serilog.Context;


namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin
{
    using Serialization;
    using SignalModel;

    [WaveformFilePlugIn("Plugin for parsing 5G NR .rfws files.", "19.1", "20")]
    public class NrRfwsPlugin : IWaveformFilePlugin
    {
        const string XmlIdentifer = "NR Generation";
        const int XmlNrVersion = 3;

        string filePath;
        XElement rootData;

        public NrRfwsPlugin() { }
        public NrRfwsPlugin(ILogger logger)
        {
            Log.Logger = logger;
        }

        public bool CanParse(WaveformConfigFileType file)
        {
            using (LogContext.PushProperty("Plugin", nameof(NrRfwsPlugin)))
            {
                // No point in continuing if it is a TDMS file
                if (file is RfwsFile)
                {
                    filePath = file.FilePath;

                    try
                    {
                        rootData = XElement.Load(filePath);
                        var result = from element in rootData.Descendants("section")
                                     where (string)element.Attribute("name") == XmlIdentifer
                                     select element;

                        bool parseable = result.FirstOrDefault() != null;
                        Log.Verbose("CanParse returning {Result} indicating that tag {XmlIdentifer} was or was not found",
                                        parseable, XmlIdentifer);
                        return parseable;
                    }
                    catch (Exception ex)
                    {
                        Log.Verbose(ex, "CanParse returning false because an exception occurred loading the file.");
                        return false;
                    }
                }
                else
                {
                    Log.Verbose("CanParse returning false because file is a TDMS file.");
                    return false;
                }
            }
        }

        public void Parse(WaveformConfigFileType file, RFmxInstrMX instr)
        {
            // Ensure CanParse was first called prior to executing this function
            if (rootData == null)
            {
                bool result = CanParse(file);
                if (!result) throw new InvalidOperationException($"{file.FileName} is not a valid file for this plugin.");
            }
            using (LogContext.PushProperty("Plugin", nameof(NrRfwsPlugin)))
            {
                // The RFWS file breaks up the NR configuration in two sections: a section representing carrier definitions (in which
                // the NR specific configurations are set), and then a all of the carrier sets in the waveform. These carrier sets
                // reference one of the carrier definitions and configure properties such as the frequency offset for the carrier.
                //
                // This doesn't neatly map to RFmx, so an extra step is performed after reading in these objects to then create
                // a unified object matching the RFmx layout.

                int carrierSetIndex = 0;
                List<RfwsCarrierSet> carrierSets = new List<RfwsCarrierSet>();
                foreach (XElement carrierSetSection in rootData.FindSections<RfwsCarrierSet>())
                {
                    RfwsCarrierSet set = carrierSetSection.Deserialize<RfwsCarrierSet>();
                    carrierSets.Add(set);
                }
                List<Carrier> carriers = new List<Carrier>();
                foreach (XElement carrierDefinitionSetion in rootData.FindSections<Carrier>())
                {
                    Carrier carrier = carrierDefinitionSetion.Deserialize<Carrier>();
                    carriers.Add(carrier);
                }
                // Now that we have loaded all relevant information from the file, construct the final object
                // and pass the data to the serialization engine to create the RFmx NR signal
                foreach (RfwsCarrierSet set in carrierSets)
                {
                    NrSignalModel signal = new NrSignalModel(set, carriers);
                    RFmxNRMX nrSignal = instr.CreateNRSignalConfigurationFromObject(signal, signalName: $"CarrierSet{carrierSetIndex}");
                    // Select initial measurements so RFmx doesn't complain on launch that nothing is selected
                    nrSignal.SelectMeasurements("", RFmxNRMXMeasurementTypes.Acp | RFmxNRMXMeasurementTypes.ModAcc, true);
                    // RFmx will complain in some configurations if this enabled; since the plugin identifes the RBs this uneeded
                    nrSignal.SetAutoResourceBlockDetectionEnabled("", RFmxNRMXAutoResourceBlockDetectionEnabled.False);
                    carrierSetIndex++;
                }
            }
        }
    }
}
