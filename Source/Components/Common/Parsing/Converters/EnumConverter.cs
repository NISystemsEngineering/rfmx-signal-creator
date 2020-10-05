using System;
using System.Text.RegularExpressions;

namespace NationalInstruments.Utilities.SignalCreator
{
    public class EnumConverter<T> : EnumConverter where T : struct, Enum
    {
        public sealed override object Convert(object value, Type targetType = null)
        {
            return Convert(value);
        }

        protected virtual T Convert(object value)
        {
            return (T)base.Convert(value, typeof(T));
        }


    }
    public class EnumConverter : ValueConverter
    {
        public enum StripOptions { None, Whitespace, AllNonAlphanumeric }

        public virtual StripOptions CharacterStripOptions { get; } = StripOptions.AllNonAlphanumeric;
        public override object Convert(object value, Type targetType)
        {
            if (value is string s)
            {
                // Strip all non-alphanumeric characters
                switch (CharacterStripOptions)
                {
                    case StripOptions.Whitespace:
                        s = s.Replace(" ", string.Empty);
                        break;
                    case StripOptions.AllNonAlphanumeric:
                        s = Regex.Replace(s, @"\W", string.Empty);
                        break;
                }
                try
                {
                    return Enum.Parse(targetType, s, true);
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException($"No valid enumerated value found for enum type {targetType} with value {s}");
                }
            }
            else
            {
                // If it's not a string, we'll try directly casting it (i.e. if value is an integer and the enum is an integer type)
                return base.Convert(value, targetType);
            }
            ;
        }
    }
}
