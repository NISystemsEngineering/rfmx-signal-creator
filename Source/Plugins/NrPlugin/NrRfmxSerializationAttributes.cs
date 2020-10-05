using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin
{
    /// <summary>
    /// Defines custom behavior for using the selector string when the <see cref="NrRfmxPropertyMap{T}"/> value is applied
    /// to the RFmx signal.
    /// </summary>
    public enum RfmxNrSelectorStringType
    {
        /// <summary>Specifies that the input selector string should be left as is.</summary>
        Default,
        /// <summary>Specifies that the subblock string should be parsed out of the selector string.</summary>
        Subblock,
        /// <summary>Specifies that the selector string should be set to an empty value.</summary>
        None
    }

    public sealed class RFmxNrMappablePropertyAttribute : RFmxMappablePropertyAttribute
    {
        public RfmxNrSelectorStringType SelectorStringType = RfmxNrSelectorStringType.Default;

        public RFmxNrMappablePropertyAttribute(RFmxNRMXPropertyId property)
            : base((int)property)
        { 

        }
        public RFmxNrMappablePropertyAttribute(RFmxNRMXPropertyId property, RfmxNrSelectorStringType selectorStringType)
            : base((int)property)
        {
            SelectorStringType = selectorStringType;
        }
    }
}
