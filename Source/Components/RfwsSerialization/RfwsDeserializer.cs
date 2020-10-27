using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Serilog;

namespace NationalInstruments.Utilities.SignalCreator.Serialization
{
    public class RfwsDeserializer : Deserializer<XElement>
    {
        protected override object ReadValue(Type t, XElement section, DeserializableAttribute attr)
        {
            switch (attr)
            {
                case RfwsDeserializableKeyAttribute keyAttr:
                    object value = section.ReadKeyValue(keyAttr.Key);
                    if (t == typeof(double))
                    {
                        return RfwsParserUtilities.ParseSiNotationDouble((string)value);
                    }
                    else return value;
                case RfwsDeserializableSectionAttribute sectionAttr:
                    object returnValue;
                    if (t.IsArray || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        returnValue =  section.FindSections(sectionAttr);
                    }
                    else returnValue = section.FindSections(sectionAttr).FirstOrDefault();

                    if (returnValue == null)
                    {
                        Log.Warning("Section {SectionName} with minimum version {SectionVersion} was not found in the file; section will be skipped. " +
                            "Save the RFWS file in the latest released version of the RFmx Waveform Creator to ensure all properties are set correctly",
                            sectionAttr.sectionName, sectionAttr.minimumSupportedVersion);
                    }

                    return returnValue;
                default: return null;
            }
        }
        protected override bool SelectValidAttribute(IEnumerable<DeserializableAttribute> attributes, XElement section, out DeserializableAttribute validAttr)
        {
            if (attributes.First() is RfwsDeserializableKeyAttribute)
        {
            float sectionVersion = section.GetSectionVersion();
                    validAttr = attributes.Cast<RfwsDeserializableKeyAttribute>().Where(attr => attr.IsSupported(sectionVersion)).FirstOrDefault();
                    return validAttr != null;
            }
            else return base.SelectValidAttribute(attributes, section, out validAttr);
        }
    }
    public static class RfwsParserExtensions
    {
        /// <summary>
        /// Deserializes data from <paramref name="element"/> to a new object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Specifies the type of object to be deserialized from the XML data.</typeparam>
        /// <returns></returns>
        public static T Deserialize<T>(this XElement element)
        {
            RfwsDeserializer parser = new RfwsDeserializer();
            return parser.Deserialize<T>(element);
        }
    }
}
