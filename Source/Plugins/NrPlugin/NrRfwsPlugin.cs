using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.NRMX;
using System.Xml.Linq;
using Serilog;
using Serilog.Context;


namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{
    using static RfwsParserUtilities;

    [WaveformFilePlugIn("Plugin for parsing 5G NR .rfws files.", "19.1", "20")]
    public class NrRfwsPlugin : IWaveformFilePlugin
    {
        const string XmlIdentifer = "NR Generation";
        const int XmlNrVersion = 3;

        string filePath;
        XElement rootData;

        public NrRfwsPlugin()
        {

        }
        public NrRfwsPlugin(ILogger logger)
        {
            Log.Logger = logger;
        }

        public bool CanParse(WaveformConfigFileType file)
        {
            using (LogContext.PushProperty("Plugin", nameof(NrRfwsPlugin)))
            {
                // No point in continuing if it is a TDMS file
                if (file is TdmsFile)
                {
                    Log.Verbose("CanParse returning false because file is a TDMS file.");
                    return false;
                }

                filePath = file.FilePath;

                try
                {
                    rootData = XElement.Load(filePath);
                    var result = from element in rootData.Descendants("section")
                                 where (string)element.Attribute("name") == XmlIdentifer
                                 select element;

                    bool parseable = result.FirstOrDefault() != null;
                    Log.Verbose("CanParse returning {Result} indicating that tag {XmlIdentifer} was or was not found",
                                    parseable, XmlIdentifer, XmlNrVersion);
                    return parseable;
                }
                catch (Exception ex)
                {
                    Log.Verbose(ex, "CanParse returning false because an exception occurred loading the file.");
                    return false;
                }
            }
        }

        public void Parse(WaveformConfigFileType file, RFmxInstrMX instr)
        {
            using (LogContext.PushProperty("Plugin", nameof(NrRfwsPlugin)))
            {
                int carrierSetIndex = 0;
                foreach (XElement carrierSetSection in FindSections(rootData, typeof(CarrierSet)))
                {
                    RFmxNRMX signal = instr.GetNRSignalConfiguration($"CarrierSet{carrierSetIndex}");

                    // Select initial measurements so RFmx doesn't complain on launch that nothing is selected
                    signal.SelectMeasurements("", RFmxNRMXMeasurementTypes.Acp | RFmxNRMXMeasurementTypes.ModAcc, true);
                    // RFmx will complain in some configurations if this enabled; since the plugin identifes the RBs this uneeded
                    signal.SetAutoResourceBlockDetectionEnabled("", RFmxNRMXAutoResourceBlockDetectionEnabled.False);

                    using (LogContext.PushProperty("CarrierSet", carrierSetIndex))
                    {
                        RfwsParser parser = new RfwsParser();
                        NrRFmxMapper nrMapper = new NrRFmxMapper(signal);

                        CarrierSet carrierSet = new CarrierSet(carrierSetSection);
                        parser.Parse(carrierSet);

                        // The carrier sets identify which carrier definition is associated with that subblcok.
                        // Hence, for each carrier definition that we find, we determine which subblock object
                        // uses that carrier definition and we use that subblock as the parent object to create
                        // the carrier.
                        int i = 0;
                        List<Carrier> carriers = new List<Carrier>();
                        foreach (XElement carrierDefinitionSetion in FindSections(rootData, typeof(Carrier)))
                        {
                            var matchingSections = from Subblock s in carrierSet.Subblocks
                                                   where s.CarrierDefinitionIndex == i
                                                   select s;
                            foreach(Subblock subblock in matchingSections)
                            {
                                Carrier c = new Carrier(carrierDefinitionSetion, subblock);
                                parser.Parse(c);
                                carriers.Add(c);
                            }
                            i++;
                        }

                        // Everything has been parsed; now, map the results
                        nrMapper.Map(carrierSet);
                        foreach (Carrier c in carriers) nrMapper.Map(c);
                    }
                }
            }
        }
    }
}
