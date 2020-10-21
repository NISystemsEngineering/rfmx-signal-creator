using System;
using NationalInstruments.RFToolkits.Interop;
using NationalInstruments.RFmx.WlanMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.WlanPlugin
{
    using Serialization;

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    class WlanTkParseableAttribute : DeserializableAttribute
    {
        public niWLANGProperties WlanGPropertyId { get; }
        public WlanTkParseableAttribute(niWLANGProperties wlanGPropertyId)
        {
            WlanGPropertyId = wlanGPropertyId;
        }
    }
    class RFmxWlanSerializablePropertyAttribute : RFmxSerializablePropertyAttribute
    {
        public RFmxWlanMXPropertyId WlanPropertyId { get; }
        public RFmxWlanSerializablePropertyAttribute(RFmxWlanMXPropertyId property)
            : base((int)property) 
        {
            WlanPropertyId = property;
        }
    }
}
