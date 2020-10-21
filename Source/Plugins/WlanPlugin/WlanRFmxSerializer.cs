using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.WlanMX;
using Serilog;

namespace NationalInstruments.Utilities.SignalCreator.Plugins
{
    using NationalInstruments.Utilities.SignalCreator.Seralization;
    using NationalInstruments.Utilities.SignalCreator;

    public class WlanRFmxMapper : RFmxSerializer<RFmxWlanMX>
    {
        static void LogKey(RFmxMappablePropertyAttribute attribute, string selectorString, object value)
        {
            RFmxWlanMappableAttribute wlanAttr = (RFmxWlanMappableAttribute)attribute;
            if (string.IsNullOrEmpty(selectorString)) selectorString = "<signal>";
            Log.Verbose("Set property {RfmxPropertyID} for {SelectorString} with value {Value}",
                wlanAttr.WlanPropertyId, selectorString, value);
        }

        protected override void ApplyConfiguration(RFmxWlanMX signal, string selectorString, bool value, RFmxMappablePropertyAttribute attribute)
        {
            LogKey(attribute, selectorString, value);
            signal.SetAttributeBool(selectorString, attribute.RFmxPropertyId, value);
        }

        protected override void ApplyConfiguration(RFmxWlanMX signal, string selectorString, double value, RFmxMappablePropertyAttribute attribute)
        {
            LogKey(attribute, selectorString, value);
            signal.SetAttributeDouble(selectorString, attribute.RFmxPropertyId, value);
        }

        protected override void ApplyConfiguration(RFmxWlanMX signal, string selectorString, int value, RFmxMappablePropertyAttribute attribute)
        {
            LogKey(attribute, selectorString, value);
            signal.SetAttributeInt(selectorString, attribute.RFmxPropertyId, value);
        }

        protected override void ApplyConfiguration(RFmxWlanMX signal, string selectorString, Enum value, RFmxMappablePropertyAttribute attribute)
        {
            LogKey(attribute, selectorString, value);
            int convertedValue = Convert.ToInt32(value);
            signal.SetAttributeInt(selectorString, attribute.RFmxPropertyId, convertedValue);
        }

        protected override void ApplyConfiguration(RFmxWlanMX signal, string selectorString, string value, RFmxMappablePropertyAttribute attribute)
        {
            LogKey(attribute, selectorString, value);
            signal.SetAttributeString(selectorString, attribute.RFmxPropertyId, value);
        }
    }
    public static class RFmxWlanMXSerializerExtensions
    {
        public static RFmxWlanMX CreateWlanSignalConfigurationFromObject(this RFmxInstrMX instr, object objectToSerialize, string baseSelectorString = "")
        {
            return instr.CreateWlanSignalConfigurationFromObject(objectToSerialize, string.Empty, baseSelectorString);
        }
        public static RFmxWlanMX CreateWlanSignalConfigurationFromObject(this RFmxInstrMX instr, object objectToSerialize, string signalName, string baseSelectorString = "")
        {
            RFmxWlanMX signal;
            if (string.IsNullOrEmpty(signalName))
            {
                signal = instr.GetWlanSignalConfiguration();
            }
            else signal = instr.GetWlanSignalConfiguration(signalName);
            WlanRFmxMapper mapper = new WlanRFmxMapper();
            mapper.Serialize(signal, baseSelectorString, objectToSerialize);
            return signal;
        }
    }
}
