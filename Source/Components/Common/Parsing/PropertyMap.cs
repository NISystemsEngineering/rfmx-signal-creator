using System;

namespace NationalInstruments.Utilities.WaveformParsing
{
    /// <summary>
    /// Represents a single RFmx property and the information needed to translate it from the incoming value to the value
    /// expected by RFmx.
    /// </summary>
    /// <typeparam name="T">Specifies the type of the value of the key. Valid types are <see cref="int"/>, <see cref="double"/>, 
    /// <see cref="bool"/>, and <see cref="string"/>.
    /// This type will be used to select the appropriate RFmx function in order to apply the property.</typeparam>
    public class PropertyMap<T>
    {
        private T value;

        /// <summary>
        /// Specifies the RFmx property ID required to set this property.
        /// </summary>
        public int RfmxPropertyId;
        /// <summary>
        /// An optional delegate to translate the raw input value to the proper type and value expected by RFmx.
        /// This is necessary if the property cannot be directly mapped to the RFmx value. If this delegate is not specified (i.e. null)
        /// the raw input value will be parsed directly to the type specified by <see cref="T"/>.
        /// <para></para>
        /// For example, this is necessary mapping a raw string value to a specific RFmx enum. 
        /// </summary>
        public Func<object, T> CustomMap;

        /// <summary>
        /// Indicates whether a value has been set to <see cref="Value"/>; the default is false.
        /// </summary>
        public bool HasValue { get; private set; } = false;
        /// <summary>
        /// Represents the value of the key after having been parsed from the input source. If the value has not been set,
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
