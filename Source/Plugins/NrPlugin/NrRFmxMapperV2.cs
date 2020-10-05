using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NationalInstruments.RFmx.NRMX;
using NationalInstruments.RFmx.InstrMX;
using Serilog;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin
{
    using NationalInstruments.Utilities.SignalCreator;
    using SignalCreator.RfwsParser;
    using SignalCreator.Seralization;

    /// <summary>
    /// Represents a mapper to apply values to an RFmx NR session.
    /// </summary>
    public class NrRFmxMapperV2 : RFmxSerializer<RFmxNRMX>
    {

        #region Overrides
        protected override void ApplyConfiguration(RFmxNRMX signal, string selectorString, bool value, RFmxMappablePropertyAttribute attribute)
        {
            string overridenSelectorString = OverrideSelectorString(attribute, selectorString);
            LogKey(attribute, selectorString, value);
            signal.SetAttributeBool(overridenSelectorString, attribute.RFmxPropertyId, value);
        }

        protected override void ApplyConfiguration(RFmxNRMX signal, string selectorString, double value, RFmxMappablePropertyAttribute attribute)
        {
            string overridenSelectorString = OverrideSelectorString(attribute, selectorString);
            LogKey(attribute, selectorString, value);
            signal.SetAttributeDouble(overridenSelectorString, attribute.RFmxPropertyId, value);
        }

        protected override void ApplyConfiguration(RFmxNRMX signal, string selectorString, int value, RFmxMappablePropertyAttribute attribute)
        {
            string overridenSelectorString = OverrideSelectorString(attribute, selectorString);
            LogKey(attribute, selectorString, value);
            signal.SetAttributeInt(overridenSelectorString, attribute.RFmxPropertyId, value);
        }
        protected override void ApplyConfiguration(RFmxNRMX signal, string selectorString, Enum value, RFmxMappablePropertyAttribute attribute)
        {
            string overridenSelectorString = OverrideSelectorString(attribute, selectorString);
            LogKey(attribute, selectorString, value);
            int convertedValue = Convert.ToInt32(value);
            signal.SetAttributeInt(overridenSelectorString, attribute.RFmxPropertyId, convertedValue);
        }
        protected override void ApplyConfiguration(RFmxNRMX signal, string selectorString, string value, RFmxMappablePropertyAttribute attribute)
        {
            string overridenSelectorString = OverrideSelectorString(attribute, selectorString);
            LogKey(attribute, selectorString, value);
            signal.SetAttributeString(overridenSelectorString, attribute.RFmxPropertyId, value);
        }
        #endregion
        static void LogKey(RFmxMappablePropertyAttribute attribute, string selectorString, object value)
        {
            RFmxNRMXPropertyId id = (RFmxNRMXPropertyId)attribute.RFmxPropertyId;
            if (string.IsNullOrEmpty(selectorString)) selectorString = "<signal>";
            Log.Verbose("Set property {RfmxPropertyID} for {SelectorString} with value {Value}",
                id, selectorString, value);
        }

        /// <summary>
        /// Overrides the incoming selector string when the <see cref="RfmxNrSelectorStringType"/> value is not 
        /// <see cref="RfmxNrSelectorStringType.Default"/>.
        /// <para></para>
        /// This is used in a few corner cases where a property is defined in one 
        /// section of an RFWS file but must use a different selector string than the other properties.
        /// </summary>
        protected static string OverrideSelectorString(RFmxMappablePropertyAttribute attribute, string selectorString)
        {
            var nrAttribute = (RFmxNrMappablePropertyAttribute)attribute;
            switch (nrAttribute.SelectorStringType)
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
    public static class RFmxNRmxSerializerExtensions
    {
        public static RFmxNRMX CreateNRSignalConfigurationFromObject(this RFmxInstrMX instr, object objectToSerialize, string baseSelectorString = "")
        {
            return instr.CreateNRSignalConfigurationFromObject(objectToSerialize, string.Empty, baseSelectorString);
        }
        public static RFmxNRMX CreateNRSignalConfigurationFromObject(this RFmxInstrMX instr, object objectToSerialize, string signalName, string baseSelectorString = "")
        {
            RFmxNRMX signal;
            if (string.IsNullOrEmpty(signalName))
            {
                signal = instr.GetNRSignalConfiguration();
            }
            else signal = instr.GetNRSignalConfiguration(signalName);
            NrRFmxMapperV2 mapper = new NrRFmxMapperV2();
            mapper.Serialize(signal, baseSelectorString, objectToSerialize);
            return signal;
        }
    }
}
