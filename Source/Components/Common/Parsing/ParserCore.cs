using Serilog;
using Serilog.Context;
using System;
using System.Reflection;

namespace NationalInstruments.Utilities.WaveformParsing
{
    /// <summary>
    /// Represents the core parsing functionality reading values from the input and parsing those values to 
    /// apply to the mapped properties in the <see cref="PropertyGroup"/>. 
    /// <para></para>
    /// Concrete implementations will define how the values are read from the input source and how they are parsed.
    /// </summary>
    public abstract class ParserCore
    {
        #region Parse Functions
        /// <summary>
        /// Reads the raw value for each <see cref="PropertyMap{T}"/> object in <paramref name="group"/> from the input source,
        /// then parses the value and applies the parsed value to the object.
        /// </summary>
        /// <param name="group"></param>
        public virtual void Parse(PropertyGroup group)
        {
            using (LogContext.PushProperty("Group", group.GetType().Name))
            {
                foreach ((FieldInfo field, object propertyMap) in group.MappedFields)
                {
                    try
                    {
                        // Allow child classes to determine whether this field is valid - i.e. matches expected version
                        if (ValidateProperty(group, field))
                        {
                            try
                            {
                                object rawValue = ReadValueFromInput(group, field);
                                Log.Verbose("Read property {Property} with value {Value}", field.Name, rawValue);
                                try
                                {
                                    ParseAndApplyValue(propertyMap, rawValue);
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex, "Error parsing property {Property}; attempted to parse {Value} but operation failed",
                                        field.Name, rawValue);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "Failed to fetch value for {Property}", field.Name);
                            }
                        }
                        else
                        {
                            Log.Debug("{FieldName} skipped because it did not pass the validation check.", field.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error validating property {Property}", field.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Parses <paramref name="rawValue"/> and applies the parsed value to <paramref name="propertyMap"/>.
        /// </summary>
        /// <param name="propertyMap">Specifies the <see cref="PropertyMap{T}"/> object to set the value to.</param>
        /// <param name="rawValue">Specifies the raw value read from the input source for the property.</param>
        private void ParseAndApplyValue(object propertyMap, object rawValue)
        {
            switch (propertyMap)
            {
                case PropertyMap<bool> boolProperty:
                    // If delegate is not set, then just directly parse the value
                    if (boolProperty.CustomMap == null)
                        boolProperty.Value = ParseValue<bool>(rawValue);
                    // Otherwise, invoke the delgate to manually map the value
                    else
                        boolProperty.Value = boolProperty.CustomMap(rawValue);
                    break;
                case PropertyMap<double> doubleProperty:
                    // If delegate is not set, then just directly parse the value
                    if (doubleProperty.CustomMap == null)
                        doubleProperty.Value = ParseValue<double>(rawValue);
                    // Otherwise, invoke the delgate to manually map the value
                    else
                        doubleProperty.Value = doubleProperty.CustomMap(rawValue);
                    break;
                case PropertyMap<int> intProperty:
                    // If delegate is not set, then just directly parse the value
                    if (intProperty.CustomMap == null)
                        intProperty.Value = ParseValue<int>(rawValue);
                    // Otherwise, invoke the delgate to manually map the value
                    else
                        intProperty.Value = intProperty.CustomMap(rawValue);
                    break;
                case PropertyMap<string> stringProperty:
                    // If delegate is not set, then just directly pass the value
                    if (stringProperty.CustomMap == null)
                        stringProperty.Value = ParseValue<string>(rawValue);
                    // Otherwise, invoke the delgate to manually map the value
                    else
                        stringProperty.Value = stringProperty.CustomMap(rawValue);
                    break;
                default:
                    throw new NotImplementedException($"Key type {propertyMap.GetType()} not supported");
            }
        }

        /// <summary>
        /// A required override for child classes to implement to define how the <see cref="PropertyMap{T}"/> object
        /// in <paramref name="field"/> should be read from the input source. Returns an object representing the value
        /// read from the input source.
        /// </summary>
        protected abstract object ReadValueFromInput(PropertyGroup group, FieldInfo field);

        /// <summary>
        /// An optional method for child classes to override if validation should be performed on a specific
        /// property. Return true if the property should be parsed and false if it should be skipped.
        /// </summary>
        protected virtual bool ValidateProperty(PropertyGroup group, FieldInfo field) => true;

        /// <summary>
        /// An optional method for child classes to override to define custom parsing for values read from the input source
        /// if they cannot be directly cast to the expected type.
        /// </summary>
        /// <typeparam name="T">Specififes the expected type of <paramref name="value"/></typeparam>
        /// <param name="value">Specifies the object to apply custom parsing to.</param>
        /// <returns></returns>
        protected virtual T ParseValue<T>(object value) => (T)value;
        #endregion

    }
}
