using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.RFToolkits.Interop;
using NationalInstruments.RFmx.WlanMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    class WlanTkParseableAttribute : ParseableAttribute
    {
        public niWLANGProperties WlanGPropertyId { get; }

        public WlanTkParseableAttribute(niWLANGProperties wlanGPropertyId)
        {
            WlanGPropertyId = wlanGPropertyId;
        }
    }
    class RFmxWlanMappableAttribute : RFmxMappablePropertyAttribute
    {
        public RFmxWlanMXPropertyId WlanPropertyId { get; }
        public RFmxWlanMappableAttribute(RFmxWlanMXPropertyId property)
            : base((int)property) 
        {
            WlanPropertyId = property;
        }
    }
}
