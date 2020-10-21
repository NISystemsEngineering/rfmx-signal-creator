using System;

namespace NationalInstruments.Utilities.SignalCreator.Serialization
{
    /// <summary>
    /// Indicates that a property or field can be deserialized. Child implementations describe how the property or field 
    /// maps to the specified data source.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public abstract class DeserializableAttribute : Attribute
    {
        /// <summary>
        /// Optional; specifies the type of <see cref="ValueConverter"/> that is used to convert this property or field from the
        /// raw data from the data container to the appropriate type of the property or field. <para></para>
        /// The default conversion methods will be used when this is null.
        /// </summary>
        public virtual Type ConverterType { get; set; } = null;
    }
}
