using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace NationalInstruments.Utilities.SignalCreator
{
    /// <summary>
    /// Represents the core parsing functionality reading values from the input and parsing those values to 
    /// apply to the mapped properties in the <see cref="PropertyGroup"/>. 
    /// <para></para>
    /// Concrete implementations will define how the values are read from the input source and how they are parsed.
    /// </summary>
    public abstract class ParserCore
    {
        //private static readonly Type[] validFieldTypes = { typeof(int), typeof(double), typeof(string), typeof(bool), typeof(Enum) };

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
                foreach ((MemberInfo member, List<ParseableAttribute> attributes) in group.ParseableMembers)
                {
                    Type t = member.GetMemberType();
                    /*if (!validFieldTypes.Any(type => type == t))
                    {
                        Log.Error("Parseable field {FieldName} is of invalid type {Type}. Valid field types are {Types}",
                            field.Name, t, validFieldTypes);
                        continue;
                    }*/
                    Type underlyingType = Nullable.GetUnderlyingType(t);

                    t = underlyingType == null ? t : underlyingType;
                    try
                    {
                        // Allow child classes to determine whether this member is valid - i.e. matches expected version
                        ParseableAttribute validAttribute = attributes.FirstOrDefault(attr => ValidateProperty(group, member, attr));
                        if (validAttribute != null)
                        {
                            try
                            {
                                object rawValue = ReadValueFromInput(group, validAttribute);
                                Log.Verbose("Read property {Property} with value {Value}", member.Name, rawValue);
                                try
                                {
                                    Type converterType = validAttribute.ConverterType == null ? typeof(ValueConverter) : validAttribute.ConverterType;
                                    ValueConverter c = (ValueConverter)Activator.CreateInstance(converterType);
                                    object o = c.Convert(rawValue, t);
                                    member.SetValue(group, o);
                                    //ParseAndApplyValue(propertyMap, rawValue);
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex, "Error parsing property {Property}; attempted to parse {Value} but operation failed",
                                        member.Name, rawValue);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "Failed to fetch value for {MemberName}", member.Name);
                            }
                        }
                        else
                        {
                            Log.Verbose("{MemberName} skipped because it did not pass the validation check.", member.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error validating property {MemberName}", member.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Parses <paramref name="rawValue"/> and applies the parsed value to <paramref name="propertyMap"/>.
        /// </summary>
        /// <param name="propertyMap">Specifies the <see cref="PropertyMap{T}"/> object to set the value to.</param>
        /// <param name="rawValue">Specifies the raw value read from the input source for the property.</param>
        private void ParseAndApplyValue(PropertyMapCore propertyMap, object rawValue)
        {
            switch (propertyMap)
            {
                case BoolPropertyMap boolProperty:
                    // If delegate is not set, then just directly parse the value
                    if (boolProperty.CustomMap == null)
                        boolProperty.Value = ParseValue<bool>(rawValue);
                    // Otherwise, invoke the delgate to manually map the value
                    else
                        boolProperty.Value = boolProperty.CustomMap(rawValue);
                    break;
                case DoublePropertyMap doubleProperty:
                    // If delegate is not set, then just directly parse the value
                    if (doubleProperty.CustomMap == null)
                        doubleProperty.Value = ParseValue<double>(rawValue);
                    // Otherwise, invoke the delgate to manually map the value
                    else
                        doubleProperty.Value = doubleProperty.CustomMap(rawValue);
                    break;
                case IntPropertyMap intProperty:
                    // If delegate is not set, then just directly parse the value
                    if (intProperty.CustomMap == null)
                        intProperty.Value = ParseValue<int>(rawValue);
                    // Otherwise, invoke the delgate to manually map the value
                    else
                        intProperty.Value = intProperty.CustomMap(rawValue);
                    break;
                case StringPropertyMap stringProperty:
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
        protected abstract object ReadValueFromInput(PropertyGroup group, ParseableAttribute attribute);

        /// <summary>
        /// An optional method for child classes to override if validation should be performed on a specific
        /// property. Return true if the property should be parsed and false if it should be skipped.
        /// </summary>
        protected virtual bool ValidateProperty(PropertyGroup group, MemberInfo member, ParseableAttribute attribute) => true;

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
