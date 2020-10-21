using System;
using NationalInstruments.RFToolkits.Interop;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.WlanPlugin
{
    using Serialization;

    internal class WlanTkDeserializer : Deserializer<niWLANG>
    {
        protected override object ReadValue(Type t, niWLANG wlan, DeserializableAttribute attr)
        {
            WlanTkParseableAttribute wlanAttr = (WlanTkParseableAttribute)attr;
            if (t == typeof(double))
            {
                wlan.GetScalarAttributeF64("", wlanAttr.WlanGPropertyId, out double doubleValue);
                return doubleValue;
            }
            else
            {
                wlan.GetScalarAttributeI32("", wlanAttr.WlanGPropertyId, out int intValue);
                return intValue;
            }
        }
    }
    internal static class WlanParserExtensions
    {
        public static T Deserialize<T>(this niWLANG wlan)
        {
            WlanTkDeserializer wlanTkDeserializer = new WlanTkDeserializer();
            return wlanTkDeserializer.Deserialize<T>(wlan);
        }
    }
}
