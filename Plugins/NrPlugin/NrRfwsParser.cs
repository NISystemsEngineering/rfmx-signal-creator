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

    public enum RfmxSelectorStringType
    {
        Default,
        Subblock,
        None
    }
    
    [WaveformFilePlugIn]
    public class NrRfwsParser : RfwsParser<RFmxNRMX>, IWaveformFilePlugin
    {
        const string XmlIdentifer = "NR Generation";
        const int XmlNrVersion = 3;

        const string SectionCarrierSet = "CarrierSet";

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
            foreach (XElement carrierSetSection in FindSections(rootData, SectionCarrierSet))
            {
                RFmxNRMX signal = instr.GetNRSignalConfiguration($"CarrierSet{carrierSetIndex}");

                signal.SelectMeasurements("", RFmxNRMXMeasurementTypes.Acp | RFmxNRMXMeasurementTypes.ModAcc, true);

                Console.WriteLine("/******************************************/");
                Console.WriteLine($"Configuring carrier set {carrierSetIndex}");
                Console.WriteLine("/******************************************/");

                CarrierSet carrierSet = new CarrierSet(rootData, carrierSetSection, this, signal, "");
                carrierSet.Parse();

                Console.WriteLine("/******************************************/");
            }
        }


        public override void ApplyConfiguration(RFmxNRMX signal, string selectorString, RfwsKey key, bool value)
        {
            selectorString = OverrideSelectorString(key, selectorString);
            signal.SetAttributeBool(selectorString, key.RfmxPropertyId, value);
        }
        public override void ApplyConfiguration(RFmxNRMX signal, string selectorString, RfwsKey key, double value)
        {
            selectorString = OverrideSelectorString(key, selectorString);
            signal.SetAttributeDouble(selectorString, key.RfmxPropertyId, value);
        }
        public override void ApplyConfiguration(RFmxNRMX signal, string selectorString, RfwsKey key, int value)
        {
            selectorString = OverrideSelectorString(key, selectorString);
            signal.SetAttributeInt(selectorString, key.RfmxPropertyId, value);
        }
        public override void ApplyConfiguration(RFmxNRMX signal, string selectorString, RfwsKey key, string value)
        {
            selectorString = OverrideSelectorString(key, selectorString);
            signal.SetAttributeString(selectorString, key.RfmxPropertyId, value);
        }

        public static string OverrideSelectorString(RfwsKey key, string selectorString)
        {
            NrRfwsKey nrkey = (NrRfwsKey)key;
            switch (nrkey.SelectorStringType)
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
