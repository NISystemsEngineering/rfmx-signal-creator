using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using NationalInstruments.RFmx.InstrMX;
using System.Text.RegularExpressions;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins

{
    public delegate void PreParsingAction<T>(XElement section, T signal, string selectorString);
    public abstract class RfwsParser<T> where T : ISignalConfiguration
    {
        public bool SkipVersionCheck { get; set; } = false;


        public void ParseAndMapProperties(RfwsSection<T> section)
        {
            #region Parse Keys
            //IEnumerable<RfwsRfmxPropertyMap> propertyMaps;
            // Deconstruct incoming object
            (XElement element, T signal, string selectorString) = section;

            float sectionVersion = float.Parse(element.Attribute("version").Value);

            var keyMapPairs = from field in section.GetType().GetFields() // Get all configured fields for the ipnut type
                              where field.IsDefined(typeof(RfwsPropertyAttribute)) // Limit fields to those implementing appropriate property
                              let attribute = field.GetCustomAttribute<RfwsPropertyAttribute>() // Save the custom attribute object in "attribute"
                              where RfwsParserUtilities.CheckMatchedVersions(sectionVersion, attribute)
                              let map = (RfwsRfmxPropertyMap)field.GetValue(null) // Save the value of the static field (aka the map defined at edit time) in "mapValue"
                              select (attribute, map); // Return the key map pair

            foreach ((RfwsPropertyAttribute attr, RfwsRfmxPropertyMap map) in keyMapPairs)
            {
                string value = RfwsParserUtilities.FetchValue(element, attr.Key);
                Console.WriteLine($"Parsed key \"{attr.Key}\" with value {value}");
                try
                {
                    switch (map.RfmxType)
                    {
                        case RmfxPropertyTypes.Bool:
                            bool parsedBool;
                            // If delegate is not set, then just directly parse the value
                            if (map.CustomMap == null)
                                parsedBool = bool.Parse(value);
                            // Otherwise, invoke the delgate to manually map the value
                            else
                                parsedBool = (bool)map.CustomMap(value);

                            ApplyConfiguration(signal, selectorString, map, parsedBool);
                            break;
                        case RmfxPropertyTypes.Double:
                            double parsedDouble;
                            // If delegate is not set, then just directly parse the value
                            if (map.CustomMap == null)
                                parsedDouble = RfwsParserUtilities.SiNotationToStandard(value);
                            // Otherwise, invoke the delgate to manually map the value
                            else
                                parsedDouble = (double)map.CustomMap(value);

                            ApplyConfiguration(signal, selectorString, map, parsedDouble);
                            break;
                        case RmfxPropertyTypes.Int:
                            int parsedInt;
                            // If delegate is not set, then just directly parse the value
                            if (map.CustomMap == null)
                                parsedInt = int.Parse(value);
                            // Otherwise, invoke the delgate to manually map the value
                            else
                                parsedInt = (int)map.CustomMap(value);

                            ApplyConfiguration(signal, selectorString, map, parsedInt);
                            break;
                        case RmfxPropertyTypes.String:
                            string parsedString;
                            // If delegate is not set, then just directly pass the value
                            if (map.CustomMap == null)
                                parsedString = value;
                            // Otherwise, invoke the delgate to manually map the value
                            else
                                parsedString = (string)map.CustomMap(value);

                            ApplyConfiguration(signal, selectorString, map, parsedString);
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
            var subSections = from type in section.GetType().GetNestedTypes()
                              where type.IsDefined(typeof(RfwsSectionAttribute))
                              let attribute = type.GetCustomAttribute<RfwsSectionAttribute>()
                              select (attribute, type);
            foreach ((RfwsSectionAttribute attr, Type sectionType) in subSections)
            {
                IEnumerable<XElement> subSection = RfwsParserUtilities.FindSections(element, attr.sectionName, attr.regExMatch);
                foreach (XElement matchedSection in subSection)
                {
                    Console.WriteLine($"Starting section {attr.sectionName}");
                    RfwsSection<T> newSection = (RfwsSection<T>)Activator.CreateInstance(sectionType, section.DocumentRoot, matchedSection, signal, selectorString);
                    ParseAndMapProperties(newSection);
                }
            }
            #endregion
        }
        public abstract void ApplyConfiguration(T signal, string selectorString, RfwsRfmxPropertyMap map, bool value);
        public abstract void ApplyConfiguration(T signal, string selectorString, RfwsRfmxPropertyMap map, double value);
        public abstract void ApplyConfiguration(T signal, string selectorString, RfwsRfmxPropertyMap map, int value);
        public abstract void ApplyConfiguration(T signal, string selectorString, RfwsRfmxPropertyMap map, string value);
    }
    public static class RfwsParserUtilities
    {
        public static IEnumerable<XElement> FindSections(XElement root, string sectionName, bool regexMatch = false)
        {
            if (regexMatch)
            {
                return from element in root.Descendants("section")
                       let name = (string)element.Attribute("name")
                       where Regex.IsMatch(name, sectionName)
                       select element;
            }
            else
            {
                return from element in root.Descendants("section")
                       let name = (string)element.Attribute("name")
                       where name == sectionName
                       select element;
            }
        }
        public static string FetchValue(XElement element, string keyName)
        {
            string result = (from child in element.Descendants()
                             where (string)child.Attribute("name") == keyName
                             select child.Value).FirstOrDefault();

            /*result = element
                    .Descendants("key")
                    .Where(e => (string)e.Attribute("name") == keyName)
                    .Where(e => versions.Contains((string)e.Parent.Attribute("version"))
                    .Select(e => e.Value)
                    .FirstOrDefault();*/


            /*var result2 = from child in element.Descendants("key")
                         where (string)child.Attribute("name") == keyName
                         where (string)child.Parent.Attribute("version") == version
                         select element.Value.ToArray();*/

            if (string.IsNullOrEmpty(result))
                throw new KeyNotFoundException($"Property named \"{keyName}\" not found.");

            return result;
        }
        public static bool CheckMatchedVersions(float sectionVersion, RfwsPropertyAttribute attribute)
        {
            switch (attribute.VersionMode)
            {
                case RfswVersionMode.AllVersions:
                    return true;
                case RfswVersionMode.SpecificVersions:
                    return attribute.Versions.Contains(sectionVersion);
                case RfswVersionMode.SupportedVersionsAndLater:
                    return (from supportedVersion in attribute.Versions
                            select sectionVersion >= supportedVersion)
                            .Aggregate( (current, next) => current |= next);
                default: 
                    return false;
            }
        }

        private static Dictionary<char, double> SiDictionary = new Dictionary<char, double>
        {
            ['p'] = 1e-9,
            ['u'] = 1e-6,
            ['m'] = 1e-3,
            ['k'] = 1e3,
            ['M'] = 1e6,
            ['G'] = 1e9
        };
        public static double SiNotationToStandard(string valueToConvert)
        {
            if (double.TryParse(valueToConvert, out double result))
                return result;

            else
            {
                foreach (var pair in SiDictionary)
                {
                    int suffixLoc = valueToConvert.IndexOf(pair.Key);
                    if (suffixLoc > 0)
                    {
                        string split = valueToConvert.Remove(suffixLoc, 1);
                        if (double.TryParse(split, out double separatedResult))
                        {
                            return separatedResult * pair.Value;
                        }
                    }
                }
                throw new ArgumentException($"Value of \"{valueToConvert}\" does not match any SI suffixes and cannot be converted.");
            }
        }

        public static T StringToEnum<T>(string value) where T : Enum
        {
            // Strip whitespace
            value = value.Replace(" ", string.Empty);
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }

}
