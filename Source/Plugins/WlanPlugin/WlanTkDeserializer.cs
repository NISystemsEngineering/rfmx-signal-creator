using System;

using System.Reflection;

using NationalInstruments.RFToolkits.Interop;

namespace NationalInstruments.Utilities.SignalCreator.Plugins
{
    class WlanParser : ParserCorev2
    {
        public object Parse(Type type, niWLANG wlan)
        {
            return ParseValue(type, wlan);
        }
        public T Parse<T>(niWLANG wlan)
        {
            return (T)Parse(typeof(T), wlan);
        }

        public override object ReadValue(Type t, object valueToParse, ParseableAttribute attr)
        {
            WlanTkParseableAttribute wlanAttr = (WlanTkParseableAttribute)attr;
            niWLANG wlan = (niWLANG)valueToParse;
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
    public static class WlansParserExtensions
    {
        public static T Parse<T>(this niWLANG wlan)
        {
            WlanParser parser = new WlanParser();
            return parser.Parse<T>(wlan);
        }
    }
}
