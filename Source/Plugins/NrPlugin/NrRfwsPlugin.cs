using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.NRMX;
using System.Xml.Linq;
using Serilog;
using Serilog.Context;

using static NationalInstruments.Utilities.WaveformParsing.Plugins.RfwsParserUtilities;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{

    [WaveformFilePlugIn("Plugin for parsing 5G NR .rfws files.", "19.1", "20")]
    public class NrRfwsPlugin : IWaveformFilePlugin
    {
        const string XmlIdentifer = "NR Generation";
        const int XmlNrVersion = 3;

        string filePath;
        XElement rootData;

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
                    var result = rootData.Descendants("section")
                        .Where(e => (string)e.Attribute("name") == XmlIdentifer && (int)e.Attribute("version") == XmlNrVersion)
                        .First();
                    bool parseable = result != null;
                    Log.Verbose("CanParse returning {Result} indicating that tag {XmlIdentifer} with version {Version} was or was not found",
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

                    signal.SelectMeasurements("", RFmxNRMXMeasurementTypes.Acp | RFmxNRMXMeasurementTypes.ModAcc, true);

                    using (LogContext.PushProperty("CarrierSet", carrierSetIndex))
                    {
                        RfwsParser parser = new RfwsParser();
                        NrRFmxMapper nrMapper = new NrRFmxMapper();

                        CarrierSet carrierSet = new CarrierSet(rootData, carrierSetSection, signal, "");
                        var carrierSets = parser.ParseSectionAndKeys(carrierSet);

                        var carrierConfigurations = new List<RfwsSection<RFmxNRMX>>();

                        int i = 0;
                        foreach (XElement carrierDefinitionSetion in FindSections(rootData, typeof(Carrier)))
                        {
                            var matchingSections = carrierSets.Where(p => p is CarrierSet.Subblock sub && sub.CarrierDefinitionIndex == i);
                            foreach (var matchedSection in matchingSections)
                            {
                                using (LogContext.PushProperty("Carrier", matchedSection.SelectorString))
                                {
                                    //Console.WriteLine($"Configuring {matchedSection.SelectorString}");
                                    Carrier c = new Carrier(carrierDefinitionSetion, matchedSection);
                                    carrierConfigurations.AddRange(parser.ParseSectionAndKeys(c));
                                }
                            }
                            i++;
                        }

                        var allParsedSections = new List<RfwsSection<RFmxNRMX>>(carrierSets.Union(carrierConfigurations));


                        foreach (var section in allParsedSections)
                        {
                            nrMapper.MapSection(section);
                        }
                    }
                }
            }
        }
    }
}
