using System;
using System.Reflection;
using System.Linq;
using System.Xml.Linq;
using Serilog;
using System.Collections.Generic;
using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.Utilities.WaveformParsing
{
    /// <summary>
    /// Represents a section (tag "section") contained within an RFWS file and the RFmx signal and selector string
    /// required to configure it.
    /// </summary>
    public abstract class ParsingGroup
    {

        private List<FieldValuePair> _mappedFields;
        public IEnumerable<FieldValuePair> MappedFields
        {
            get
            {
                if (_mappedFields == null)
                {
                    var fields = from field in GetType().GetFields() // Get all configured fields for the input type
                                 let type = field.FieldType
                                 where type.IsSubclassOfRawGeneric(typeof(RFmxPropertyMap<>))
                                 let fieldPair = new FieldValuePair(field, this) // Save the value of the field (aka the map defined at edit time)
                                 select fieldPair; // Return the key map pair
                    _mappedFields = fields.ToList();
                }
                return _mappedFields.AsReadOnly();
            }
        }
        public virtual string SelectorString { get; }

        protected ParsingGroup(string selectorString)
        {
            SelectorString = selectorString;
        }

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

