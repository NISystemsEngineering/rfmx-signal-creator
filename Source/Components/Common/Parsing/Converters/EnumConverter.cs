using System;

namespace NationalInstruments.Utilities.SignalCreator
{
    public class EnumConverter<T> : ValueConverter<T> where T : struct, Enum
    {
        protected override T Convert(object value)
        {
            if (value is string s)
            {
                // Strip whitespace
                s = s.Replace(" ", string.Empty);
                bool valid = Enum.TryParse(s, true, out T result);
                if (!valid)
                {
                    throw new ArgumentException($"No valid enumerated value found for enum type {typeof(T)} with value {s}");
                }
                
                return result;
            }
            else
            {
                // If it's not a string, we'll try directly casting it (i.e. if value is an integer and the enum is an integer type)
                return base.Convert(value);
            }
        }
    }
}
