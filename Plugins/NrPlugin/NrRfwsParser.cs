using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.NRMX;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using static NationalInstruments.Utilities.WaveformParsing.Plugins.RfwsParserUtilities;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{
    public class NrRfwsParser : RfwsParser<RFmxNRMX>, IWaveformFilePlugin
    {
        const string XmlIdentifer = "NR Generation";
        const int XmlNrVersion = 3;

        const string SectionCarrierSet = "CarrierSet";
        const string SectionCarrier = "Carrier";

        public RFmxInstrMX Instr { get;  }
        public RFmxNRMX[] Signals { get; }

        string filePath;
        XElement rootData;

        public bool CanParse(WaveformConfigFile file)
        {
            filePath = file.FilePath;

            try
            {
                rootData = XElement.Load(filePath);
                var result = rootData.Descendants("section")
                    .Where(e => (string)e.Attribute("name") == XmlIdentifer && (int)e.Attribute("version") == XmlNrVersion)
                    .First();
                return result != null;
            }
            catch
            {
                return false;
            }
        }

        public void Parse(WaveformConfigFile file, RFmxInstrMX instr)
        {
            int carrierSetIndex = 0;
            foreach(XElement carrierSet in FindSections(rootData, SectionCarrierSet))
            {
                RFmxNRMX signal = instr.GetNRSignalConfiguration($"CarrierSet{carrierSetIndex}");

                Console.WriteLine("/******************************************/");
                Console.WriteLine($"Configuring carrier set {carrierSetIndex}");
                Console.WriteLine("/******************************************/");

                foreach(XElement subblock in FindSections(carrierSet, SectionCarrier))
                {
                    try
                    {

                        Subblock s = new Subblock(rootData, subblock, signal, "");
                        ParseAndMapProperties(s);

                        Carrier c = new Carrier(rootData, s.SectionRoot, s.Signal, s.SelectorString);
                        ParseAndMapProperties(c);
                        /*
                        // Setup: Subblock
                        string selectorString = "";
                        
                        // Build selector string for the current subblock and CC
                        int subblockIndex = int.Parse(FetchValue(subblock, Subblock.KeySubblockNumber));

                        signal.SetNumberOfSubblocks("", subblockIndex + 1);
                        
                        selectorString = RFmxNRMX.BuildSubblockString(selectorString, subblockIndex);
                        
                        // RFmx WC defines the subblocks only in relative frequency offsets
                        signal.SetSubblockFrequencyDefinition(selectorString, RFmxNRMXSubblockFrequencyDefinition.Relative);

                        int ccIndex = int.Parse(FetchValue(subblock, Subblock.KeyCarrierCCIndex));
                        signal.ComponentCarrier.SetNumberOfComponentCarriers(selectorString, ccIndex + 1);
                        
                        selectorString = RFmxNRMX.BuildCarrierString(selectorString, ccIndex);


                        Console.WriteLine($"Configuring subblock {subblockIndex}, component carrier {ccIndex}");

                        ParseAndMapProperties(typeof(Subblock), subblock, signal, selectorString);

                        // Setup carrier

                        // Fetch the carrier definition number in order to reference the appropriate carrier in a moment
                        string carrierDefinitionIndex = FetchValue(subblock, Subblock.KeyCarrierDefinition);

                        // Fetch the carrier definition section
                        XElement carrierDefinition = FindSections(rootData, "CarrierDefinitionManager").First();
                        // Fetch the specific carrier
                        XElement specificCarrier = FindSections(carrierDefinition, carrierDefinitionIndex).First();

                        ParseAndMapProperties(typeof(Carrier), specificCarrier, signal, selectorString);
                        */
                    }
                    catch (Exception ex)
                    {
                        throw new FormatException($"Error parsing subblock.", ex);
                    }
                }

                carrierSetIndex++;
                Console.WriteLine("/******************************************/");
            }
        }


        public override void ApplyConfiguration(RFmxNRMX signal, string selectorString, RfwsRfmxPropertyMap map, bool value)
        {
            selectorString = OverrideSelectorString(map, selectorString);
            signal.SetAttributeBool(selectorString, map.RfmxPropertyId, value);
        }
        public override void ApplyConfiguration(RFmxNRMX signal, string selectorString, RfwsRfmxPropertyMap map, double value)
        {
            selectorString = OverrideSelectorString(map, selectorString);
            signal.SetAttributeDouble(selectorString, map.RfmxPropertyId, value);
        }
        public override void ApplyConfiguration(RFmxNRMX signal, string selectorString, RfwsRfmxPropertyMap map, int value)
        {
            selectorString = OverrideSelectorString(map, selectorString);
            signal.SetAttributeInt(selectorString, map.RfmxPropertyId, value);
        }
        public override void ApplyConfiguration(RFmxNRMX signal, string selectorString, RfwsRfmxPropertyMap map, string value)
        {
            selectorString = OverrideSelectorString(map, selectorString);
            signal.SetAttributeString(selectorString, map.RfmxPropertyId, value);
        }

        public static string OverrideSelectorString(RfwsRfmxPropertyMap map, string selectorString)
        {
            NrRfwsRfmxPropertyMap nrMap = (NrRfwsRfmxPropertyMap)map;
            switch (nrMap.SelectorStringType)
            {
                case RfmxSelectorStringType.Subblock:
                    Match result = Regex.Match(selectorString, @"subblock\d+");
                    if (result.Success) return result.Value;
                    break;
                case RfmxSelectorStringType.None:
                    return string.Empty;
            }
            return selectorString;
        }
    }
}
