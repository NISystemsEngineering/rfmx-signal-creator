using System;

namespace NationalInstruments.Utilities.SignalCreator
{
    public class ValueConverter
    {
        public virtual object Convert(object value, Type targetType)
        {
            return System.Convert.ChangeType(value, targetType);
        }
    }
}
