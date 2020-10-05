using System;

namespace NationalInstruments.Utilities.SignalCreator.RfwsParser.Converters
{
    public class DefaultRfwsConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType)
        {
            if (targetType == typeof(double))
            {
                return RfwsParserUtilities.ParseSiNotationDouble((string)value);
            }
            else return base.Convert(value, targetType);
        }
    }
}
