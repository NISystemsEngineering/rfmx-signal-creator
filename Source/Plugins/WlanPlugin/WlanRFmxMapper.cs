using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.RFmx.WlanMX;
using Serilog;

namespace NationalInstruments.Utilities.SignalCreator.Plugins
{
    public class WlanRFmxMapper : MapperCore<RFmxWlanMX>
    {
        public WlanRFmxMapper(RFmxWlanMX signal) 
            : base(signal) { }

        protected override void ApplyConfiguration(string selectorString, PropertyMap<bool> property)
        {
            LogKey(property, selectorString);
            Signal.SetAttributeBool(selectorString, property.RfmxPropertyId, property.Value);
        }

        protected override void ApplyConfiguration(string selectorString, PropertyMap<double> property)
        {
            LogKey(property, selectorString);
            Signal.SetAttributeDouble(selectorString, property.RfmxPropertyId, property.Value);
        }

        protected override void ApplyConfiguration(string selectorString, PropertyMap<int> property)
        {
            LogKey(property, selectorString);
            Signal.SetAttributeInt(selectorString, property.RfmxPropertyId, property.Value);
        }

        protected override void ApplyConfiguration(string selectorString, PropertyMap<string> property)
        {
            LogKey(property, selectorString);
            Signal.SetAttributeString(selectorString, property.RfmxPropertyId, property.Value);
        }
        static void LogKey<T>(PropertyMap<T> key, string selectorString)
        {
            RFmxWlanMXPropertyId id = (RFmxWlanMXPropertyId)key.RfmxPropertyId;
            if (string.IsNullOrEmpty(selectorString)) selectorString = "<signal>";
            Log.Verbose("Set property {RfmxPropertyID} of type {PropretyType} for {SelectorString} with value {Value}",
                id, typeof(T), selectorString, key.Value);
        }
    }
}
