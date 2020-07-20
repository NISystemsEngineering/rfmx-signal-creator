using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.RFmx.InstrMX;
using System.Xml.Linq;
using System.Reflection;
using System.IO;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{
    public abstract class RfwsSection<T> where T : ISignalConfiguration
    {
        public const string KeyVersion = "version";

        public string SelectorString { get; protected set; }
        public T Signal { get; protected set; }
        public XElement SectionRoot { get; protected set; }
        public XElement DocumentRoot { get; protected set; }
        public RfwsParser<T> RfwsParser;

        public float Version {  get => float.Parse(SectionRoot.Attribute(KeyVersion).Value); }

        public RfwsSection(XElement documentRoot, XElement section, RfwsParser<T> parser, T signal, string selectorString)
        {
            Signal = signal;
            SelectorString = selectorString;
            SectionRoot = section;
            DocumentRoot = documentRoot;
            RfwsParser = parser;
        }

        public virtual void Parse()
        {
            #region Parse Keys
            //IEnumerable<RfwsRfmxPropertyMap> propertyMaps;
            // Deconstruct incoming object
            //(XElement element, T Signal, string SelectorString) = section;

            var keys = from field in GetType().GetFields() // Get all configured fields for the ipnut type
                              where field.IsDefined(typeof(RfwsPropertyAttribute)) // Limit fields to those implementing appropriate property
                              let attribute = field.GetCustomAttribute<RfwsPropertyAttribute>() // Save the custom attribute object in "attribute"
                              where RfwsParserUtilities.CheckMatchedVersions(Version, attribute)
                              let map = (RfwsKey)field.GetValue(this) // Save the value of the static field (aka the map defined at edit time) in "mapValue"
                              select (attribute, map); // Return the key map pair

            foreach ((RfwsPropertyAttribute attr, RfwsKey key) in keys)
            {
                string value = RfwsParserUtilities.FetchValue(SectionRoot, attr.Key);
                Console.Write($"Parsed key \"{attr.Key}\" with value {value}.");
                try
                {
                    switch (key.RfmxType)
                    {
                        case RmfxPropertyTypes.Bool:
                            bool parsedBool;
                            // If delegate is not set, then just directly parse the value
                            if (key.CustomMap == null)
                                parsedBool = bool.Parse(value);
                            // Otherwise, invoke the delgate to manually map the value
                            else
                                parsedBool = (bool)key.CustomMap(value);
                            Console.WriteLine($" Setting RFmx value to {parsedBool}");
                            RfwsParser.ApplyConfiguration(Signal, SelectorString, key, parsedBool);
                            break;
                        case RmfxPropertyTypes.Double:
                            double parsedDouble;
                            // If delegate is not set, then just directly parse the value
                            if (key.CustomMap == null)
                                parsedDouble = RfwsParserUtilities.SiNotationToStandard(value);
                            // Otherwise, invoke the delgate to manually map the value
                            else
                                parsedDouble = (double)key.CustomMap(value);
                            Console.WriteLine($" Setting RFmx value to {parsedDouble}");
                            RfwsParser.ApplyConfiguration(Signal, SelectorString, key, parsedDouble);
                            break;
                        case RmfxPropertyTypes.Int:
                            int parsedInt;
                            // If delegate is not set, then just directly parse the value
                            if (key.CustomMap == null)
                                parsedInt = int.Parse(value);
                            // Otherwise, invoke the delgate to manually map the value
                            else
                                parsedInt = (int)key.CustomMap(value);
                            Console.WriteLine($" Setting RFmx value to {parsedInt}");
                            RfwsParser.ApplyConfiguration(Signal, SelectorString, key, parsedInt);
                            break;
                        case RmfxPropertyTypes.String:
                            string parsedString;
                            // If delegate is not set, then just directly pass the value
                            if (key.CustomMap == null)
                                parsedString = value;
                            // Otherwise, invoke the delgate to manually map the value
                            else
                                parsedString = (string)key.CustomMap(value);
                            Console.WriteLine($" Setting RFmx value to {parsedString}");
                            RfwsParser.ApplyConfiguration(Signal, SelectorString, key, parsedString);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                catch (Exception ex)
                {
                    throw new FileLoadException($"Error parsing key {attr.Key}.", ex);
                }
            }
            #endregion
            #region Parse Sub-Sections
            var subSections = from type in GetType().GetNestedTypes()
                              where type.IsDefined(typeof(RfwsSectionAttribute))
                              let attribute = type.GetCustomAttribute<RfwsSectionAttribute>()
                              select (attribute, type);
            foreach ((RfwsSectionAttribute attr, Type sectionType) in subSections)
            {
                IEnumerable<XElement> subSection = RfwsParserUtilities.FindSections(SectionRoot, attr.sectionName, attr.regExMatch);
                foreach (XElement matchedSection in subSection)
                {
                    Console.WriteLine($"Starting section {attr.sectionName}");
                    RfwsSection<T> newSection = (RfwsSection<T>)Activator.CreateInstance(sectionType, DocumentRoot, matchedSection, RfwsParser, Signal, SelectorString);
                    newSection.Parse();
                }
            }
            #endregion
        }

        /*public void Deconstruct(out XElement section, out T Signal, out string SelectorString)
        {
            section = SectionRoot;
            Signal = Signal;
            SelectorString = SelectorString;
        }*/
    }
}
