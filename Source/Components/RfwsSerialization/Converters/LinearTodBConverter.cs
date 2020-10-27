using System;

namespace NationalInstruments.Utilities.SignalCreator.Serialization.Converters
{
    /// <summary>
    /// Converts a linear ratio value string from Volts to decibels.
    /// </summary>
    public class LinearTodBConverter : ValueConverter<double>
    {
        public static double ValueTodB(string value)
        {
            double scalingFactor = double.Parse(value);
            return 20 * Math.Log(scalingFactor);
        }
        protected override double Convert(object value)
        {
            double linear = 0;
            switch (value)
            {
                case string stringValue:
                    linear = double.Parse(stringValue);
                    break;
                case double d:
                    linear = d;
                    break;
                default:
                    try
                    {
                        linear = (double)value;
                    }
                    catch (InvalidCastException)
                    {
                        throw new ArgumentException($"Value {value} of type {value.GetType()} cannot be converted to double.", nameof(value));
                    }
                    break;
            }
            return 20 * Math.Log(linear);
        }
    }
}
