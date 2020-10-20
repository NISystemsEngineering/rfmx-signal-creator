using System;


namespace NationalInstruments.Utilities.SignalCreator.Serialization.Converters
{
    /// <summary>
    /// A generic implementation of <see cref="ValueConverter"/> that enforces proper type handling in child implementations.
    /// </summary>
    /// <typeparam name="T">Specifies the type </typeparam>
    public abstract class ValueConverter<T> : ValueConverter
    {
        // Sealed so that child classes implement the typed version below
        public sealed override object Convert(object value, Type targetType = null)
        {
            return Convert(value);
        }
        /// <summary>
        /// Represents a typed implementation of <see cref="ValueConverter.Convert(object, Type)"/> converting <paramref name="value"/>
        /// to <typeparamref name="T"/>. <para></para>
        /// The default implementation invokes the parent using the system conversion functions.
        /// </summary>
        /// <param name="value">SPecifies the value to be converted to <typeparamref name="T"/>.</param>
        /// <returns></returns>
        protected virtual T Convert(object value)
        {
            return (T)base.Convert(value, typeof(T));
        }
    }
}
