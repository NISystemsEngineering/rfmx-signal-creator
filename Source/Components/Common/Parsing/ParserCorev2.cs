using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace NationalInstruments.Utilities.SignalCreator
{
    // Modeled on https://github.com/zanders3/json
    public abstract class ParserCorev2
    {
        /*public T Parse<T>()
        {
            return (T)ParseValue(typeof(T));
        }*/
        protected object ParseValue(Type t, object valueToParse, Type converterType = null )
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
                    return Convert.ChangeType(valueToParse, t);
                }
                if (t.IsEnum)
                {
                    if (valueToParse is string s)
                    {
                        // Strip whitespace
                        s = s.Replace(" ", string.Empty);
                        return Enum.Parse(t, s, true);
                    }
                    else
                    {
                        // If it's not a string, we'll try directly casting it (i.e. if value is an integer and the enum is an integer type)
                        return Convert.ChangeType(valueToParse, t);
                    }
                }
                if (t.IsSubclassOfRawGeneric(typeof(List<>)))
                {
                    Type listType = t.GetGenericArguments()[0];
                    var list = (IList)t.GetConstructor(Type.EmptyTypes).Invoke(null);
                    //list.Add()

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
                    return ParseObject(t, valueToParse);
                }
                return null;
            }
        }
        protected object ParseObject(Type t, object valueToParse)
        {
            object instance = FormatterServices.GetUninitializedObject(t);

            var members = t.GetPropertiesAndFields(MemberAccessibility.Writeable).Where(m => m.IsDefined(typeof(ParseableAttribute)));

            foreach (MemberInfo m in members)
            {
                var attrs = m.GetCustomAttributes<ParseableAttribute>();

                if (SelectValidAttribute(attrs, valueToParse, out ParseableAttribute attr))
                {
                    Type memberType = m.GetMemberType();
                    object rawValue = ReadValue(memberType, valueToParse, attr);
                    //Log.Verbose("Read {MemberName} with raw value {RawValue}", m.Name, rawValue);
                    object parsedValue = ParseValue(memberType, rawValue, attr.ConverterType);
                    m.SetValue(instance, parsedValue);
                    //Log.Verbose("Set member {MemberName} with parsed value {ParsedValue}", m.Name, parsedValue);
                }
            }
            return instance;
        }

        public abstract object ReadValue(Type t, object valueToParse, ParseableAttribute attr);
        public virtual bool SelectValidAttribute(IEnumerable<ParseableAttribute> attributes, object valueToParse, out ParseableAttribute validAttr)
        {
            validAttr = attributes.First();
            return true;
        }
    }
}
