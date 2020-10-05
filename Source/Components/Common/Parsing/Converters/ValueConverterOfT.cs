using System;


namespace NationalInstruments.Utilities.SignalCreator
{
    public abstract class ValueConverter<T> : ValueConverter
    {
        public sealed override object Convert(object value, Type targetType = null)
        {
            return Convert(value);
        }

        protected virtual T Convert(object value)
        {
            try
            {
                T result = (T)value;
                return result;
            }
            catch (InvalidCastException e)
            {
                throw new ArgumentException($"Value {value} cannot be converted to type {typeof(T)}", e);
            }
        }
    }
}
