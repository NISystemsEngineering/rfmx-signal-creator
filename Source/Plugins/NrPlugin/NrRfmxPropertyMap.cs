namespace NationalInstruments.Utilities.WaveformParsing.Plugins
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
    /// <typeparam name="T"></typeparam>
    public class NrRfmxPropertyMap<T> : PropertyMap<T>
    {
        /// <summary>
        /// Specifies how the selector string should be used when this property is mapped.
        /// </summary>
        public RfmxNrSelectorStringType SelectorStringType;
    }
}
