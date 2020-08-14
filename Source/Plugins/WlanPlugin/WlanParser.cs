using System;

using System.Reflection;

using NationalInstruments.RFToolkits.Interop;

namespace NationalInstruments.Utilities.SignalCreator.Plugins
{
    class WlanParser : ParserCore
    {
        private niWLANG wlan;
        
        public WlanParser(niWLANG wlan)
        {
            this.wlan = wlan;
        }

        protected override object ReadValueFromInput(PropertyGroup group, FieldInfo field)
        {
            if (field.IsDefined(typeof(WlanPropertyAttribute)))
            {
                WlanPropertyAttribute attr = field.GetCustomAttribute<WlanPropertyAttribute>();

                object map = field.GetValue(group);
                if (map is PropertyMap<double>)
                {
                    wlan.GetScalarAttributeF64("", attr.WlanGPropertyId, out double doubleValue);
                    return doubleValue;
                }
                else
                {
                    wlan.GetScalarAttributeI32("", attr.WlanGPropertyId, out int intValue);
                    return intValue;
                }
            }
            else
            {
                throw new CustomAttributeFormatException($"{typeof(WlanPropertyAttribute)}" +
                    $"must be defined for {field.Name}");
            }
        }
    }
}
