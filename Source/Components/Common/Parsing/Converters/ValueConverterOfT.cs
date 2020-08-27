using System;


namespace NationalInstruments.Utilities.SignalCreator
{
    public abstract class ValueConverter<T> : ValueConverter
    {
        public sealed override object Convert(object value, Type targetType = null)
        {
            return Convert(value);
        }

        protected abstract T Convert(object value);
    }
}
