using NationalInstruments.RFmx.InstrMX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NationalInstruments.Utilities.SignalCreator
{
    /// <summary>
    /// Represents a group of <see cref="PropertyMap{T}"/> objects and the RFmx selector string required to appropriately configure the properties.
    /// <para></para>
    /// In order to be parsed correctly, <see cref="PropertyMap{T}"/> objects must be implemented as public fields.
    /// </summary>
    public abstract class PropertyGroup
    {
        private List<(MemberInfo, List<RFmxMappableAttribute>)> _mappableMembers;
        private List<(MemberInfo, List<ParseableAttribute>)> _parseableMembers;


        /// <summary>
        /// Returns all of the public <see cref="PropertyMap{T}"/> fields contained within the object and the values of those 
        /// fields if non-null.
        /// </summary>
        public IEnumerable<(MemberInfo member, List<ParseableAttribute> attribute)> ParseableMembers
            => _parseableMembers;
        public IEnumerable<(MemberInfo member, List<RFmxMappableAttribute> attribute)> MappableMembers => _mappableMembers;

        //public IEnumerable<PropertyMapCore> AllPropertyMaps => _mappedFields.Select(f => f.property);


        /// <summary>Specifies the RFmx selector string required to configure the properties contianed within this property group.</summary>
        public virtual string SelectorString { get; }

        protected PropertyGroup(string selectorString)
        {
            SelectorString = selectorString;

            var fields = GetType().GetFields().Cast<MemberInfo>();
            var readableProperties = GetType().GetProperties()
                                              .Where(property => property.GetGetMethod() != null)
                                              .Cast<MemberInfo>();
            var writeableProperteies = GetType().GetProperties()
                                                .Where(property => property.GetSetMethod() != null)
                                                .Cast<MemberInfo>();

            var parseableMembers = from member in writeableProperteies.Concat(fields) // Get all configured fields for the input type
                                   where member.IsDefined(typeof(ParseableAttribute))
                                   let attributes = member.GetCustomAttributes<ParseableAttribute>().ToList()
                                   //where type.IsSubclassOf(typeof(PropertyMapCore)) // Retrieve all PropertyMaps regardless of specific type
                                   //let value = (PropertyMapCore)field.GetValue(this)
                                   //where value != null // The value should be set by default in the class; if not, we can't do anything with it so skip it
                                   select (member, attributes); // Return the field and its value as a paired tuple
            _parseableMembers = parseableMembers.ToList();

            var mappableMembers = from member in readableProperties.Concat(fields) // Get all configured fields for the input type
                                  where member.IsDefined(typeof(RFmxMappableAttribute))
                                  let attributes = member.GetCustomAttributes<RFmxMappableAttribute>().ToList()
                                  select (member, attributes); // Return the field and its value as a paired tuple
            _mappableMembers = mappableMembers.ToList();
        }

        /// <summary>
        /// An optional method allowing a group to perform manual configurations on <paramref name="signal"/> during execution of 
        /// <see cref="MapperCore{T}.Map(PropertyGroup)"/>. <para></para>
        /// This is only necessary to override in situations where RFmx configurations must be applied that do not map directly to a property
        /// from the waveform configuration file.
        /// </summary>
        /// <param name="signal">Specifies the RFmx signal that will be used for custom configuration.</param>
        public virtual void CustomConfigure(ISignalConfiguration signal) { }

    }

   [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public abstract class ParseableAttribute : Attribute
    {
    }

    public static class Extensions
    {

        public static object GetValue(this MemberInfo member, object containingObject)
        {
            switch (member)
            {
                case FieldInfo f:
                    return f.GetValue(containingObject);
                case PropertyInfo p:
                    return p.GetValue(containingObject);
                default:
                    throw new ArgumentException($"Paramter must be a {typeof(FieldInfo)} or a {typeof(MemberInfo)}.", nameof(member));
            }
        }
        public static void SetValue(this MemberInfo member, object containingObject, object value)
        {
            switch (member)
            {
                case FieldInfo f:
                    f.SetValue(containingObject, value);
                    break;
                case PropertyInfo p:
                    p.SetValue(containingObject, value);
                    break;
                default:
                    throw new ArgumentException($"Paramter must be a {typeof(FieldInfo)} or a {typeof(MemberInfo)}.", nameof(member));
            }
        }
        public static Type GetMemberType(this MemberInfo member)
        {
            switch (member)
            {
                case FieldInfo f:
                    return f.FieldType;
                case PropertyInfo p:
                    return p.PropertyType;
                default:
                    throw new ArgumentException($"Paramter must be a {typeof(FieldInfo)} or a {typeof(MemberInfo)}.", nameof(member));
            }
        }



        // From https://www.extensionmethod.net/csharp/type/issubclassofrawgeneric
        /// <summary>
        /// Alternative version of <see cref="Type.IsSubclassOf"/> that supports raw generic types (generic types without
        /// any type parameters).
        /// </summary>
        /// <param name="baseType">The base type class for which the check is made.</param>
        /// <param name="toCheck">To type to determine for whether it derives from <paramref name="baseType"/>.</param>
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type baseType)
        {
            while (toCheck != typeof(object))
            {
                Type cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (baseType == cur)
                {
                    return true;
                }

                toCheck = toCheck.BaseType;
            }

            return false;
        }
        public virtual Type ConverterType { get; set; }
    }
}

