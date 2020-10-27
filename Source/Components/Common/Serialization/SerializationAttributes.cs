using System;

namespace NationalInstruments.Utilities.SignalCreator.Serialization
{
    public abstract class RFmxSerializableAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class RFmxSerializablePropertyAttribute : RFmxSerializableAttribute
    {
        public int RFmxPropertyId { get; }

        public RFmxSerializablePropertyAttribute(int RFmxPropertyId)
        {
            this.RFmxPropertyId = RFmxPropertyId;
        }
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class RFmxSerializableSectionAttribute : RFmxSerializableAttribute
    {
        public string SelectorString { get; }
        public RFmxSerializableSectionAttribute()
        {
            SelectorString = "";
        }
        public RFmxSerializableSectionAttribute(string selectorString)
        {
            SelectorString = selectorString;
        }
    }
}
