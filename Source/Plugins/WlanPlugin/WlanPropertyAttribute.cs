using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.RFToolkits.Interop;

namespace NationalInstruments.Utilities.SignalCreator.Plugins
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    class WlanPropertyAttribute : Attribute
    {
        public niWLANGProperties WlanGPropertyId { get; }

        public WlanPropertyAttribute(niWLANGProperties wlanGPropertyId)
        {
            WlanGPropertyId = wlanGPropertyId;
        }
    }
}
