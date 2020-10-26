using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Serilog;
using Serilog.Context;

namespace NationalInstruments.Utilities.SignalCreator.Serialization
{
    using Converters;

    // Modeled on https://github.com/zanders3/json
    /// <summary>
    /// Represents a generic solution for deserializing objects of type <typeparamref name="TDataContainer"/>.
    /// </summary>
    /// <typeparam name="TDataContainer">Specifies the type of data sources from which objects can be deserialized.</typeparam>
    public abstract class Deserializer<TDataContainer>
    {
        /// <summary>
        /// Deserialzes <paramref name="dataContainer"/> and returns an object of type <paramref name="t"/>.
        /// </summary>
        /// <param name="t">Specifies the type of object to deserialize from <paramref name="dataContainer"/>.</param>
        /// <param name="dataContainer">Specifies the object containing the data to be deserialized.</param>
        /// <returns></returns>
        public object Deserialize(Type t, TDataContainer dataContainer)
        {
            return ParseValue(t, dataContainer);
        }
        /// <summary>
        /// Deserialzes <paramref name="dataContainer"/> and returns an object of type <paramref name="t"/>.
        /// </summary>
        /// <typeparam name="T">Specifies the type of object to deserialize from <paramref name="dataContainer"/>.</typeparam>
        /// <param name="dataContainer">Specifies the object containing the data to be deserialized.</param>
        /// <returns></returns>
        public T Deserialize<T>(TDataContainer dataContainer)
        {
            return (T)Deserialize(typeof(T), dataContainer);
        }


        protected object ParseValue(Type t, object valueToParse, Type converterType = null)
        {
            t = Nullable.GetUnderlyingType(t) ?? t;
            if (converterType != null)
            {
                ValueConverter c = (ValueConverter)Activator.CreateInstance(converterType);
                return c.Convert(valueToParse, t);
            }
            else
            {
                if (t.IsPrimitive || t == typeof(string))
                {
                    ValueConverter converter = new ValueConverter();
                    return converter.Convert(valueToParse, t);
                }
                if (t.IsEnum)
                {
                    EnumConverter converter = new EnumConverter();
                    return converter.Convert(valueToParse, t);
                }
                if (t.IsSubclassOfRawGeneric(typeof(List<>)))
                {
                    Type listType = t.GetGenericArguments()[0];
                    var list = (IList)t.GetConstructor(Type.EmptyTypes).Invoke(null);

                    if (valueToParse is IEnumerable enumerable)
                    {
                        foreach (object rawValue in enumerable)
                        {
                            object parsedValue = ParseValue(listType, rawValue);
                            list.Add(parsedValue);
                        }
                        return list;
                    }
                    else
                    {
                        throw new ArgumentException("Value must implement the IEnumerable interface if the type is a collection.", nameof(valueToParse));
                    }
                }
                if (t.IsClass)
                {
                    return ParseObject(t, (TDataContainer)valueToParse);
                }
                return null;
            }
        }
        protected object ParseObject(Type t, TDataContainer dataContainer)
        {
            using (LogContext.PushProperty("Object", t.Name))
            {
                object instance = FormatterServices.GetUninitializedObject(t);

                var members = t.GetPropertiesAndFields(MemberAccessibility.Writeable).Where(m => m.IsDefined(typeof(DeserializableAttribute)));

                foreach (MemberInfo m in members)
                {
                    using (LogContext.PushProperty("Member", m.Name))
                    {
                        var attrs = m.GetCustomAttributes<DeserializableAttribute>();
                        if (SelectValidAttribute(attrs, dataContainer, out DeserializableAttribute attr))
                        {
                            Type memberType = m.GetMemberType();
                            memberType = Nullable.GetUnderlyingType(memberType) ?? memberType;
                            object rawValue = default;
                            
                            // We don't want to dump to the log objects and other things who we will parse element-by-element
                            // in a moment; hence, just limit it to simple value types.
                            bool log = memberType.IsPrimitive || memberType.IsEnum || memberType == typeof(string);

                            try
                            {
                                rawValue = ReadValue(memberType, dataContainer, attr);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "Error reading raw member value from file");
                                continue;
                            }
                            if (rawValue != null)
                            {
                                object parsedValue = default;
                                try
                                {
                                    parsedValue = ParseValue(memberType, rawValue, attr.ConverterType);
                                    if (log) Log.Verbose("Parsed raw value of {RawValue} to {ParsedValue} for member", rawValue, parsedValue);
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex, "Error parsing member");
                                }
                                if (memberType.IsAssignableFrom(parsedValue.GetType()))
                                {
                                    m.SetValue(instance, parsedValue);
                                }
                                else
                                {
                                    Log.Error("A value of {RawValue} was read from the input and parsed to the value of {ParsedValue} succesfully. " +
                                        "However, the member type of {MemberType} is not assignable from the type of {ParsedType}.",
                                        rawValue, parsedValue, memberType, parsedValue.GetType());
                                }
                            }
                            
                        }
                        else
                        {
                            Log.Debug("Member skipped because it did not pass the validation check.");
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// In a child class, reads a value of type <paramref name="t"/> from <paramref name="dataContainer"/> using deserialization information in 
        /// <paramref name="attr"/>.
        /// </summary>
        /// <param name="t">Specifies the type of object to be read from <paramref name="dataContainer"/>.</param>
        /// <param name="dataContainer">Specifies the object containing the data.</param>
        /// <param name="attr">Specifies the deserialization atttribute descrbing how data should be read.</param>
        /// <returns></returns>
        protected abstract object ReadValue(Type t, TDataContainer dataContainer, DeserializableAttribute attr);
        /// <summary>
        /// Allows a child class to implement selection of a valid deserialization attribute in the case where more than one is specified. The default
        /// implementation for this method is to select the first attribute.
        /// </summary>
        /// <param name="attributes">Specifies a collection of deserialization attributes specified by the member.</param>
        /// <param name="dataContainer">Specifies the data container from which data is read.</param>
        /// <param name="validAttr">Returns the attribute that has been selected.</param>
        /// <returns></returns>
        protected virtual bool SelectValidAttribute(IEnumerable<DeserializableAttribute> attributes, TDataContainer dataContainer, out DeserializableAttribute validAttr)
        {
            validAttr = attributes.First();
            return true;
        }
    }
}
