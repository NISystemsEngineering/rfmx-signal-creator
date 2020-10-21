using System;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.WlanMX;
using Serilog;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.WlanPlugin
{
    using Serialization;

    internal class WlanRFmxSerializer : RFmxSerializer<RFmxWlanMX>
    {
        static void LogKey(RFmxSerializablePropertyAttribute attribute, string selectorString, object value)
        {
            RFmxWlanSerializablePropertyAttribute wlanAttr = (RFmxWlanSerializablePropertyAttribute)attribute;
            if (string.IsNullOrEmpty(selectorString)) selectorString = "<signal>";
            Log.Verbose("Set property {RfmxPropertyID} for {SelectorString} with value {Value}",
                wlanAttr.WlanPropertyId, selectorString, value);
        }

        protected override void ApplyConfiguration(RFmxWlanMX signal, string selectorString, bool value, RFmxSerializablePropertyAttribute attribute)
        {
            LogKey(attribute, selectorString, value);
            signal.SetAttributeBool(selectorString, attribute.RFmxPropertyId, value);
        }

        protected override void ApplyConfiguration(RFmxWlanMX signal, string selectorString, double value, RFmxSerializablePropertyAttribute attribute)
        {
            LogKey(attribute, selectorString, value);
            signal.SetAttributeDouble(selectorString, attribute.RFmxPropertyId, value);
        }

        protected override void ApplyConfiguration(RFmxWlanMX signal, string selectorString, int value, RFmxSerializablePropertyAttribute attribute)
        {
            LogKey(attribute, selectorString, value);
            signal.SetAttributeInt(selectorString, attribute.RFmxPropertyId, value);
        }

        protected override void ApplyConfiguration(RFmxWlanMX signal, string selectorString, Enum value, RFmxSerializablePropertyAttribute attribute)
        {
            LogKey(attribute, selectorString, value);
            int convertedValue = Convert.ToInt32(value);
            signal.SetAttributeInt(selectorString, attribute.RFmxPropertyId, convertedValue);
        }

        protected override void ApplyConfiguration(RFmxWlanMX signal, string selectorString, string value, RFmxSerializablePropertyAttribute attribute)
        {
            LogKey(attribute, selectorString, value);
            signal.SetAttributeString(selectorString, attribute.RFmxPropertyId, value);
        }
    }
    internal static class RFmxWlanMXSerializerExtensions
    {
        /// <summary>
        /// Creates a WLAN signal configuration by serializing the data in <paramref name="objectToSerialize"/>.
        /// </summary>
        /// <returns></returns>
        public static RFmxWlanMX CreateWlanSignalConfigurationFromObject(this RFmxInstrMX instr, object objectToSerialize, string baseSelectorString = "")
        {
            return instr.CreateWlanSignalConfigurationFromObject(objectToSerialize, string.Empty, baseSelectorString);
        }
        /// <summary>
        /// Creates an WLAN signal configuration by serializing the data in <paramref name="objectToSerialize"/>.
        /// </summary>
        public static RFmxWlanMX CreateWlanSignalConfigurationFromObject(this RFmxInstrMX instr, object objectToSerialize, string signalName, string baseSelectorString = "")
        {
            RFmxWlanMX signal;
            if (string.IsNullOrEmpty(signalName))
            {
                signal = instr.GetWlanSignalConfiguration();
            }
            else signal = instr.GetWlanSignalConfiguration(signalName);
            WlanRFmxSerializer mapper = new WlanRFmxSerializer();
            mapper.Serialize(signal, baseSelectorString, objectToSerialize);
            return signal;
        }
    }
}
