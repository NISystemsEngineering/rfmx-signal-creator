using System;
using System.Text.RegularExpressions;
using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{
    class NrRFmxMapper : RfmxMapper<RFmxNRMX>
    {
        protected override void ApplyConfiguration(RfwsSection<RFmxNRMX> section, RfwsKey<bool> key)
        {
            (RFmxNRMX signal, string selectorString) = section;
            string overridenSelectorString = OverrideSelectorString(key, selectorString);
            LogToConsole(key, overridenSelectorString);
            signal.SetAttributeBool(overridenSelectorString, key.RfmxPropertyId, key.Value);
        }

        protected override void ApplyConfiguration(RfwsSection<RFmxNRMX> section, RfwsKey<double> key)
        {
            (RFmxNRMX signal, string selectorString) = section;
            string overridenSelectorString = OverrideSelectorString(key, selectorString);
            LogToConsole(key, overridenSelectorString);

            signal.SetAttributeDouble(overridenSelectorString, key.RfmxPropertyId, key.Value);
        }

        protected override void ApplyConfiguration(RfwsSection<RFmxNRMX> section, RfwsKey<int> key)
        {
            (RFmxNRMX signal, string selectorString) = section;
            string overridenSelectorString = OverrideSelectorString(key, selectorString);
            LogToConsole(key, overridenSelectorString);
            signal.SetAttributeInt(overridenSelectorString, key.RfmxPropertyId, key.Value);
        }

        protected override void ApplyConfiguration(RfwsSection<RFmxNRMX> section, RfwsKey<string> key)
        {
            (RFmxNRMX signal, string selectorString) = section;
            string overridenSelectorString = OverrideSelectorString(key, selectorString);
            LogToConsole(key, overridenSelectorString);
            signal.SetAttributeString(overridenSelectorString, key.RfmxPropertyId, key.Value);
        }

        static void LogToConsole<T>(RfwsKey<T> key, string selectorString)
        {
            RFmxNRMXPropertyId id = (RFmxNRMXPropertyId)key.RfmxPropertyId;
            if (string.IsNullOrEmpty(selectorString)) selectorString = "<signal>";
            Console.WriteLine($"Setting attribute of type {typeof(T)} to ID {id} for {selectorString} with value of {key.Value}");
        }

        protected static string OverrideSelectorString<T>(RfwsKey<T> key, string selectorString)
        {
            var nrKey = (NrRfwsKey<T>)key;
            switch (nrKey.SelectorStringType)
            {
                case RfmxNrSelectorStringType.Subblock:
                    Match result = Regex.Match(selectorString, @"subblock\d+");
                    if (result.Success) return result.Value;
                    break;
                case RfmxNrSelectorStringType.None:
                    return string.Empty;
            }
            return selectorString;
        }
    }
}
