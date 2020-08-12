using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using NationalInstruments.RFmx.InstrMX;
using System.Text.RegularExpressions;
using Serilog;
using Serilog.Context;

namespace NationalInstruments.Utilities.WaveformParsing
{


    public class RfwsParser : ParserCore
    {
        protected sealed override bool ValidateProperty(PropertyGroup group, FieldInfo field)
        {
            string fieldName = field.Name;
            if (group is RfwsSection rfwsGroup)
            {
                RfwsPropertyAttribute attr = rfwsGroup.GetAttribute(field);
                if (attr != null)
                {
                    bool result = RfwsParserUtilities.CheckMatchedVersions(rfwsGroup.Version, attr);
                    if (!result)
                    {
                        Log.Debug("No matching version for {Key} was found for section version {Version}",
                            fieldName, rfwsGroup.Version);
                    }
                    return result;
                }
                else
                {
                    Log.Error("{Key} will not be parsed because there is no valid {Attribute} associated with it",
                        fieldName, nameof(RfwsPropertyAttribute));
                    return false;
                }
            }
            else
            {
                //throw new invalid
                Log.Error("{Key} will not be parsed because it is of invalid type; expected {Type} but found {BadType}",
                    fieldName, typeof(RfwsSection).Name, group.GetType().Name);
                return false;
            }
        }

        public override void Parse(PropertyGroup group)
        {
            if (group is RfwsSection rfwsGroup)
            {
                base.Parse(group);
                foreach (RfwsSection subGroup in rfwsGroup.SubSections)
                {
                    Parse(subGroup);
                }
            }
            else throw new InvalidDataException($"Paramter \"{nameof(group)}\" must be of type {typeof(RfwsSection)}");
        }

        protected sealed override T ParseValue<T>(object value)
        {
            Type t = typeof(T);
            string textValue = (string)value;

            object result;
            if (t == typeof(bool))
                result = bool.Parse(textValue);
            else if (t == typeof(int))
                result = int.Parse(textValue);
            else if (t == typeof(double))
                result = RfwsParserUtilities.SiNotationToStandard(textValue);
            else if (t == typeof(string))
                result = textValue;
            else
                throw new NotSupportedException();

            return (T)result;
        }
        protected sealed override object ReadValueFromInput(PropertyGroup group, FieldInfo field)
        {
            RfwsSection rfwsGroup = (RfwsSection)group;
            RfwsPropertyAttribute attr = rfwsGroup.GetAttribute(field);
            return RfwsParserUtilities.FetchValue(rfwsGroup.SectionRoot, attr.Key);
        }

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
        public static IEnumerable<XElement> FindSections(XElement parentSection, Type sectionType)
        {
            (string sectionName, string version, bool regexMatch) = sectionType.GetCustomAttribute<RfwsSectionAttribute>();

            return FindSections(parentSection, sectionName, regexMatch);
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
                            .Aggregate((current, next) => current |= next);
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
        public static T ToEnum<T>(this object value) where T : Enum
        {
            return StringToEnum<T>((string)value);
        }
        public static T ToEnum<T>(this string value) where T : Enum
        {
            return StringToEnum<T>(value);
        }
        private static T StringToEnum<T>(string value) where T : Enum
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
