using System;

namespace NationalInstruments.Utilities.SignalCreator.Serialization.Converters
{
    /// <summary>
    /// Represents a conversion operation to convert from a raw data type from the data source to a specified type.
    /// </summary>
    public class ValueConverter
    {
        /// <summary>
        /// Converts <paramref name="value"/> to <paramref name="targetType"/>. <para></para>
        /// The default implementation uses <see cref="System.Convert.ChangeType(object, System.Type)"/> to change the type. Child classes will
        /// specify more specific conversion routines.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public virtual object Convert(object value, Type targetType)
        {
            return System.Convert.ChangeType(value, targetType);
        }
    }
}
