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


    public class RfwsParser
    {
        public bool SkipVersionCheck { get; set; } = false;

        public List<RfwsSection<T>> ParseSectionAndKeys<T>(RfwsSection<T> rfwsSection)
            where T : ISignalConfiguration
        {
            #region Parse Keys
            IEnumerable<(RfwsPropertyAttribute attribute, object key)> keys = RfwsParserUtilities.FetchSectionKeys(rfwsSection);

            foreach ((RfwsPropertyAttribute attr, object key) in keys)
            {
                if (RfwsParserUtilities.CheckMatchedVersions(rfwsSection.Version, attr))
                {
                    try
                    {
                        string value = RfwsParserUtilities.FetchValue(rfwsSection.SectionRoot, attr.Key);
                        Log.Verbose("Parsed key {key} with value {value}", attr.Key, value);
                        //Console.WriteLine($"Parsed key \"{attr.Key}\" with value {value}.");

                        try
                        {
                            switch (key)
                            {
                                case RfwsKey<bool> boolKey:
                                    // If delegate is not set, then just directly parse the value
                                    if (boolKey.CustomMap == null)
                                        boolKey.Value = bool.Parse(value);
                                    // Otherwise, invoke the delgate to manually map the value
                                    else
                                        boolKey.Value = boolKey.CustomMap(value);
                                    break;
                                case RfwsKey<double> doubleKey:
                                    // If delegate is not set, then just directly parse the value
                                    if (doubleKey.CustomMap == null)
                                        doubleKey.Value = RfwsParserUtilities.SiNotationToStandard(value);
                                    // Otherwise, invoke the delgate to manually map the value
                                    else
                                        doubleKey.Value = doubleKey.CustomMap(value);
                                    break;
                                case RfwsKey<int> intKey:
                                    // If delegate is not set, then just directly parse the value
                                    if (intKey.CustomMap == null)
                                        intKey.Value = int.Parse(value);
                                    // Otherwise, invoke the delgate to manually map the value
                                    else
                                        intKey.Value = intKey.CustomMap(value);
                                    break;
                                case RfwsKey<string> stringKey:
                                    // If delegate is not set, then just directly pass the value
                                    if (stringKey.CustomMap == null)
                                        stringKey.Value = value;
                                    // Otherwise, invoke the delgate to manually map the value
                                    else
                                        stringKey.Value = stringKey.CustomMap(value);
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Error parsing key {Key}.", attr.Key);
                        }
                    }
                    catch (KeyNotFoundException k)
                    {
                        Log.Warning("Expected to find key {KeyName} but it was not found", attr.Key);
                    }
                }
                else
                {
                    Log.Debug("{KeyName} supporting version(s) {Versions} did not match section version {SectionVersion}", 
                        attr.Key, attr.Versions, rfwsSection.Version);
                }

            }
            #endregion

            var parsedSections = new List<RfwsSection<T>>();
            parsedSections.Add(rfwsSection);

            #region Parse Sub-Sections
            var subSections = from type in rfwsSection.GetType().GetNestedTypes()
                              where type.IsDefined(typeof(RfwsSectionAttribute))
                              let attribute = type.GetCustomAttribute<RfwsSectionAttribute>()
                              select (attribute, type);
            foreach ((RfwsSectionAttribute attr, Type sectionType) in subSections)
            {
                IEnumerable<XElement> subSection = RfwsParserUtilities.FindSections(rfwsSection.SectionRoot, attr.sectionName, attr.regExMatch);
                foreach (XElement matchedSection in subSection)
                {
                    using (LogContext.PushProperty("Section", attr.sectionName))
                    {
                        try
                        {
                            RfwsSection<T> newSection = (RfwsSection<T>)Activator.CreateInstance(sectionType, matchedSection, rfwsSection);
                            var parsedSubSections = ParseSectionAndKeys(newSection);
                            parsedSections.AddRange(parsedSubSections);
                        }
                        catch (MissingMethodException ex)
                        {
                            Log.Error("Unable to parse section {SectionName} due to plugin error; see log for details", attr.sectionName);
                            Log.Debug(ex, "Section has invalid constructor; expected \"XElement childSection, RfwsSection<T> parentSection\"");
                        }
                    }
                }
            }
            return parsedSections;
            #endregion
        }


    }
    public static class RfwsParserUtilities
    {
        /// <summary>
        /// Fetches all keys and their associated attributes from <paramref name="section"/> using reflection.
        /// </summary>
        /// <typeparam name="T">Specifies the RFmx signal type used by <paramref name="section"/>.</typeparam>
        /// <param name="section">Specifies the section from which to retrieve keys and attributes.</param>
        /// <returns></returns>
        public static IEnumerable<(RfwsPropertyAttribute attribute, object key)> FetchSectionKeys<T>(RfwsSection<T> section) where T : ISignalConfiguration
        {
            var keys = from field in section.GetType().GetFields() // Get all configured fields for the ipnut type
                       where field.IsDefined(typeof(RfwsPropertyAttribute)) // Limit fields to those implementing appropriate property
                       let attribute = field.GetCustomAttribute<RfwsPropertyAttribute>() // Save the custom attribute object in "attribute"
                       //where CheckMatchedVersions(section.Version, attribute)
                       let key = field.GetValue(section) // Save the value of the static field (aka the map defined at edit time) in "mapValue"
                       select (attribute, key); // Return the key map pair
            return keys;
        }
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

            if (regexMatch)
            {
                return from element in parentSection.Descendants("section")
                       let name = (string)element.Attribute("name")
                       where Regex.IsMatch(name, sectionName)
                       select element;
            }
            else
            {
                return from element in parentSection.Descendants("section")
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
