using System;

namespace NationalInstruments.Utilities.SignalCreator
{
    public class EnumConverter<T> : ValueConverter<T> where T : struct, Enum
    {
        protected override T Convert(object value)
        {
            if (value is string s)
            {
                bool valid = Enum.TryParse<T>(s, true, out T result);
                if (!valid)
                {
                    throw new ArgumentException($"No valid enumerated value found for enum type {typeof(T)} with value {s}");
                }
                
                return result;
            }
            else
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
}
