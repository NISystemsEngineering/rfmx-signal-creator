using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin
{
    using Serialization;

    /// <summary>
    /// Defines custom behavior for using the selector string when the property value is applied
    /// to the RFmx signal.
    /// </summary>
    internal enum RfmxNrSelectorStringType
    {
        /// <summary>Specifies that the input selector string should be left as is.</summary>
        Default,
        /// <summary>Specifies that the subblock string should be parsed out of the selector string.</summary>
        Subblock,
        /// <summary>Specifies that the selector string should be set to an empty value.</summary>
        None
    }

    internal sealed class RFmxNrSerializablePropertyAttribute : RFmxSerializablePropertyAttribute
    {
        public RfmxNrSelectorStringType SelectorStringType = RfmxNrSelectorStringType.Default;

        public RFmxNrSerializablePropertyAttribute(RFmxNRMXPropertyId property)
            : base((int)property)
        { 

        }
        public RFmxNrSerializablePropertyAttribute(RFmxNRMXPropertyId property, RfmxNrSelectorStringType selectorStringType)
            : base((int)property)
        {
            SelectorStringType = selectorStringType;
        }
    }
}
