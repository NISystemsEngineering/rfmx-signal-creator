using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NationalInstruments.RFmx.NRMX;
using NationalInstruments.RFmx.InstrMX;
using Serilog;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin
{
    using Serialization;

    /// <summary>
    /// Represents a mapper to apply values to an RFmx NR session.
    /// </summary>
    internal class NrRFmxSerializer : RFmxSerializer<RFmxNRMX>
    {
        #region Overrides
        protected override void ApplyConfiguration(RFmxNRMX signal, string selectorString, bool value, RFmxSerializablePropertyAttribute attribute)
        {
            string overridenSelectorString = OverrideSelectorString(attribute, selectorString);
            LogKey(attribute, selectorString, value);
            signal.SetAttributeBool(overridenSelectorString, attribute.RFmxPropertyId, value);
        }

        protected override void ApplyConfiguration(RFmxNRMX signal, string selectorString, double value, RFmxSerializablePropertyAttribute attribute)
        {
            string overridenSelectorString = OverrideSelectorString(attribute, selectorString);
            LogKey(attribute, selectorString, value);
            signal.SetAttributeDouble(overridenSelectorString, attribute.RFmxPropertyId, value);
        }

        protected override void ApplyConfiguration(RFmxNRMX signal, string selectorString, int value, RFmxSerializablePropertyAttribute attribute)
        {
            string overridenSelectorString = OverrideSelectorString(attribute, selectorString);
            LogKey(attribute, selectorString, value);
            signal.SetAttributeInt(overridenSelectorString, attribute.RFmxPropertyId, value);
        }
        protected override void ApplyConfiguration(RFmxNRMX signal, string selectorString, Enum value, RFmxSerializablePropertyAttribute attribute)
        {
            string overridenSelectorString = OverrideSelectorString(attribute, selectorString);
            LogKey(attribute, selectorString, value);
            int convertedValue = Convert.ToInt32(value);
            signal.SetAttributeInt(overridenSelectorString, attribute.RFmxPropertyId, convertedValue);
        }
        protected override void ApplyConfiguration(RFmxNRMX signal, string selectorString, string value, RFmxSerializablePropertyAttribute attribute)
        {
            string overridenSelectorString = OverrideSelectorString(attribute, selectorString);
            LogKey(attribute, selectorString, value);
            signal.SetAttributeString(overridenSelectorString, attribute.RFmxPropertyId, value);
        }
        #endregion
        static void LogKey(RFmxSerializablePropertyAttribute attribute, string selectorString, object value)
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
        protected static string OverrideSelectorString(RFmxSerializablePropertyAttribute attribute, string selectorString)
        {
            var nrAttribute = (RFmxNrSerializablePropertyAttribute)attribute;
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
    internal static class RFmxNRmxSerializerExtensions
    {
        /// <summary>
        /// Creates an NR signal configuration by serializing the data in <paramref name="objectToSerialize"/>.
        /// </summary>
        /// <returns></returns>
        public static RFmxNRMX CreateNRSignalConfigurationFromObject(this RFmxInstrMX instr, object objectToSerialize, string baseSelectorString = "")
        {
            return instr.CreateNRSignalConfigurationFromObject(objectToSerialize, string.Empty, baseSelectorString);
        }
        /// <summary>
        /// Creates an NR signal configuration by serializing the data in <paramref name="objectToSerialize"/>.
        /// </summary>
        public static RFmxNRMX CreateNRSignalConfigurationFromObject(this RFmxInstrMX instr, object objectToSerialize, string signalName, string baseSelectorString = "")
        {
            RFmxNRMX signal;
            if (string.IsNullOrEmpty(signalName))
            {
                signal = instr.GetNRSignalConfiguration();
            }
            else signal = instr.GetNRSignalConfiguration(signalName);
            NrRFmxSerializer mapper = new NrRFmxSerializer();
            mapper.Serialize(signal, baseSelectorString, objectToSerialize);
            return signal;
        }
    }
}
