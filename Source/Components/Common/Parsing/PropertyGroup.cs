using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.Utilities.SignalCreator
{
    /// <summary>
    /// Represents a group of <see cref="PropertyMap{T}"/> objects and the RFmx selector string required to appropriately configure the properties.
    /// <para></para>
    /// In order to be parsed correctly, <see cref="PropertyMap{T}"/> objects must be implemented as public fields.
    /// </summary>
    public abstract class PropertyGroup
    {

        private List<(FieldInfo, object)> _mappedFields;

        /// <summary>
        /// Returns all of the public <see cref="PropertyMap{T}"/> fields contained within the object and the values of those 
        /// fields if non-null.
        /// </summary>
        public IEnumerable<(FieldInfo field, object fieldValue)> MappedFields
        {
            get
            {
                if (_mappedFields == null)
                {
                    var fields = from field in GetType().GetFields() // Get all configured fields for the input type
                                 let type = field.FieldType
                                 where type.IsSubclassOfRawGeneric(typeof(PropertyMap<>)) // Retrieve all PropertyMaps regardless of specific type
                                 let value = field.GetValue(this)
                                 where value != null // The value should be set by default in the class; if not, we can't do anything with it so skip it
                                 select (field, value); // Return the field and its value as a paired tuple
                    _mappedFields = fields.ToList();
                }
                return _mappedFields.AsReadOnly();
            }
        }

        /// <summary>Specifies the RFmx selector string required to configure the properties contianed within this property group.</summary>
        public virtual string SelectorString { get; }

        protected PropertyGroup(string selectorString)
        {
            SelectorString = selectorString;
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

    public static class TypeExtensions
    {
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
    }
}

