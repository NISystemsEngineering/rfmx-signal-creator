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
    public abstract class RfwsParser<T> where T : ISignalConfiguration
    {
        public bool SkipVersionCheck { get; set; } = false;

        public abstract void ApplyConfiguration(T signal, string selectorString, RfwsKey key, bool value);
        public abstract void ApplyConfiguration(T signal, string selectorString, RfwsKey key, double value);
        public abstract void ApplyConfiguration(T signal, string selectorString, RfwsKey key, int value);
        public abstract void ApplyConfiguration(T signal, string selectorString, RfwsKey key, string value);
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

            if (string.IsNullOrEmpty(result))
                throw new KeyNotFoundException($"Property named \"{keyName}\" not found.");
            else
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
        public static double ValueTodB(string value)
        {
            double scalingFactor = double.Parse(value);
            return 20 * Math.Log(scalingFactor);
        }
    }

}
