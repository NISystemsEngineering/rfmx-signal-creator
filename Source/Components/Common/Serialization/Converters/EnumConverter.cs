using System;
using System.Text.RegularExpressions;

namespace NationalInstruments.Utilities.SignalCreator.Serialization.Converters
{
    /// <summary>
    /// A <see cref="ValueConverter"/> to convert string or numeric values to the appropriate enum type.
    /// </summary>
    public class EnumConverter : ValueConverter
    {
        /// <summary>
        /// Defines options for handling incoming strings when converting to enum values.
        /// </summary>
        public enum StripOptions 
        {
            /// <summary>Do not modify the incoming string.</summary>
            None,
            /// <summary>Strip whitespace from the incoming string.</summary>
            Whitespace,
            /// <summary>Strip all non-alphanumeric characters from the incoming string.</summary>
            AllNonAlphanumeric 
        }

        /// <summary>
        /// Specifies additional processing for strings when converting to enum values.
        /// </summary>
        public virtual StripOptions CharacterStripOptions { get; } = StripOptions.AllNonAlphanumeric;

        /// <summary>
        /// Converts the string or numeric <paramref name="value"/> to an enum of type <paramref name="targetType"/>.
        /// </summary>
        /// <returns></returns>
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
    /// <summary>
    /// A typed instance of <see cref="EnumConverter"/> for easier type handling. Converts strings or numerics to the enum of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumConverter<T> : EnumConverter where T : struct, Enum
    {
        public sealed override object Convert(object value, Type targetType = null)
        {
            return Convert(value);
        }
        /// <summary>
        /// Converts <paramref name="value"/> to an enum of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns></returns>
        protected virtual T Convert(object value)
        {
            return (T)base.Convert(value, typeof(T));
        }
    }
}
