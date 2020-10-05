using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalInstruments.Utilities.SignalCreator
{
    public abstract class RFmxMappableAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class RFmxMappablePropertyAttribute : RFmxMappableAttribute
    {
        public int RFmxPropertyId { get; }

        public RFmxMappablePropertyAttribute(int RFmxPropertyId)
        {
            this.RFmxPropertyId = RFmxPropertyId;
        }
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class RFmxMappableSectionAttribute : RFmxMappableAttribute
    {
        public string SelectorString { get; }
        public RFmxMappableSectionAttribute()
        {
            SelectorString = "";
        }
        public RFmxMappableSectionAttribute(string selectorString)
        {
            SelectorString = selectorString;
        }
    }
}
