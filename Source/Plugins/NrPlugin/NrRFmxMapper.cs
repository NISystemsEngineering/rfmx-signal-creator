using System;
using System.Text.RegularExpressions;
using NationalInstruments.RFmx.NRMX;
using Serilog;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{
    class NrRFmxMapper : RfmxMapper<RFmxNRMX>
    {
        public NrRFmxMapper(RFmxNRMX signal)
            : base(signal) { }

        protected override void ApplyConfiguration(RfwsSection section, RfwsKey<bool> key)
        {
            string overridenSelectorString = OverrideSelectorString(key, section.SelectorString);
            LogKey(key, overridenSelectorString);
            Signal.SetAttributeBool(overridenSelectorString, key.RfmxPropertyId, key.Value);
        }

        protected override void ApplyConfiguration(RfwsSection section, RfwsKey<double> key)
        {
            string overridenSelectorString = OverrideSelectorString(key, section.SelectorString);
            LogKey(key, overridenSelectorString);

            Signal.SetAttributeDouble(overridenSelectorString, key.RfmxPropertyId, key.Value);
        }

        protected override void ApplyConfiguration(RfwsSection section, RfwsKey<int> key)
        {
            string overridenSelectorString = OverrideSelectorString(key, section.SelectorString);
            LogKey(key, overridenSelectorString);
            Signal.SetAttributeInt(overridenSelectorString, key.RfmxPropertyId, key.Value);
        }

        protected override void ApplyConfiguration(RfwsSection section, RfwsKey<string> key)
        {
            string overridenSelectorString = OverrideSelectorString(key, section.SelectorString);
            LogKey(key, overridenSelectorString);
            Signal.SetAttributeString(overridenSelectorString, key.RfmxPropertyId, key.Value);
        }

        static void LogKey<T>(RfwsKey<T> key, string selectorString)
        {
            RFmxNRMXPropertyId id = (RFmxNRMXPropertyId)key.RfmxPropertyId;
            if (string.IsNullOrEmpty(selectorString)) selectorString = "<signal>";
            Log.Verbose("Set property {RfmxPropertyID} of type {PropretyType} for {SelectorString} with value {Value}",
                id, typeof(T), selectorString, key.Value);
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
