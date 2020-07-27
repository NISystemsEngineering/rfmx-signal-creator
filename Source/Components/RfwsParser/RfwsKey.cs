using System;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{
    /// <summary>
    /// Represents a single key contained in an RFWS file and the information needed to translate it to an RFWS 
    /// </summary>
    /// <typeparam name="T">Specifies the type of the value of the key. Valid types are int, double, bool, and string.
    /// This type will be used to select the appropriate RFmx function in order to apply the property.</typeparam>
    public abstract class RfwsKey<T>
    {
        private T value;

        /// <summary>
        /// Specifies the RFmx property ID used to set this property.
        /// </summary>
        public int RfmxPropertyId;
        /// <summary>
        /// Optional; use if the string value from the RFWS file cannot be directly mapped to the RFmx value. For example,
        /// mapping a string value to a specific RFmx enum. If this delegate is not specified (i.e. null) the value
        /// from the XML file will be parsed directly to the type specified by <see cref="T"/>.
        /// </summary>
        public Func<string, T> CustomMap;

        /// <summary>
        /// Indicates whether a value has been set to <see cref="Value"/>; the default is false.
        /// </summary>
        public bool HasValue { get; private set; } = false;
        /// <summary>
        /// Represents the value of the key after having been parsed from the RFWS file. If the value has not been set,
        /// attempting to access the property will throw an <see cref="InvalidOperationException"/>.
        /// </summary>
        public T Value
        {
            // This is a simplified version of how Nullable<T> is implemented. Nullable would be easier to use here rather
            // than creating a simplified version, but because string also must be a valid type nullable value types was not
            // an option.
            get
            {
                if (!HasValue) throw new InvalidOperationException("Value has not been set");
                else return value;
            }
            set
            {
                this.value = value;
                HasValue = true;
            }
        }
    }
}
