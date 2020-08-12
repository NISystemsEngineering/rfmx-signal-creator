using System;
using System.Text.RegularExpressions;
using NationalInstruments.RFmx.NRMX;
using Serilog;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{
    /// <summary>
    /// Represents a mapper to apply values to an RFmx NR session.
    /// </summary>
    public class NrRFmxMapper : RfwsMapper<RFmxNRMX>
    {
        public NrRFmxMapper(RFmxNRMX signal)
            : base(signal) { }

        #region Overrides
        protected override void ApplyConfiguration(string selectorString, PropertyMap<bool> key)
        {
            string overridenSelectorString = OverrideSelectorString(key, selectorString);
            LogKey(key, overridenSelectorString);
            Signal.SetAttributeBool(overridenSelectorString, key.RfmxPropertyId, key.Value);
        }

        protected override void ApplyConfiguration(string selectorString, PropertyMap<double> key)
        {
            string overridenSelectorString = OverrideSelectorString(key, selectorString);
            LogKey(key, overridenSelectorString);

            Signal.SetAttributeDouble(overridenSelectorString, key.RfmxPropertyId, key.Value);
        }

        protected override void ApplyConfiguration(string selectorString, PropertyMap<int> key)
        {
            string overridenSelectorString = OverrideSelectorString(key, selectorString);
            LogKey(key, overridenSelectorString);
            Signal.SetAttributeInt(overridenSelectorString, key.RfmxPropertyId, key.Value);
        }

        protected override void ApplyConfiguration(string selectorString, PropertyMap<string> key)
        {
            string overridenSelectorString = OverrideSelectorString(key, selectorString);
            LogKey(key, overridenSelectorString);
            Signal.SetAttributeString(overridenSelectorString, key.RfmxPropertyId, key.Value);
        }
        #endregion

        static void LogKey<T>(PropertyMap<T> key, string selectorString)
        {
            RFmxNRMXPropertyId id = (RFmxNRMXPropertyId)key.RfmxPropertyId;
            if (string.IsNullOrEmpty(selectorString)) selectorString = "<signal>";
            Log.Verbose("Set property {RfmxPropertyID} of type {PropretyType} for {SelectorString} with value {Value}",
                id, typeof(T), selectorString, key.Value);
        }

        /// <summary>
        /// Overrides the incoming selector string when the <see cref="RfmxNrSelectorStringType"/> value is not 
        /// <see cref="RfmxNrSelectorStringType.Default"/>.
        /// <para></para>
        /// This is used in a few corner cases where a property is defined in one 
        /// section of an RFWS file but must use a different selector string than the other properties.
        /// </summary>
        protected static string OverrideSelectorString<T>(PropertyMap<T> key, string selectorString)
        {
            var nrKey = (NrRfmxPropertyMap<T>)key;
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
