using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Serilog;
using Serilog.Context;

namespace NationalInstruments.Utilities.SignalCreator
{
    /// <summary>
    /// Implements parsing functionality for RFWS waveform cofniguration files.
    /// </summary>
    public class RfwsParser : ParserCore
    {
        /// <summary>
        /// Returns true if the field supports the section version of the file; false if not.
        /// </summary>
        protected sealed override bool ValidateProperty(PropertyGroup group, FieldInfo field)
        {
            string fieldName = field.Name;
            RfwsSection rfwsSection = (RfwsSection)group;

            RfwsPropertyAttribute attr = field.GetCustomAttribute<RfwsPropertyAttribute>();
            if (attr != null)
            {
                bool result = attr.IsSupported(rfwsSection.Version);
                if (!result)
                {
                    Log.Debug("No matching version for {Key} was found for section version {Version}",
                        fieldName, rfwsSection.Version);
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

        public override void Parse(PropertyGroup group)
        {
            if (group is RfwsSection rfwsGroup)
            {
                using (LogContext.PushProperty("SelectorString", rfwsGroup.SelectorString))
                {
                    base.Parse(group);
                    foreach (RfwsSection subGroup in rfwsGroup.SubSections)
                    {
                        Parse(subGroup);
                    }
                }
            }
            else
            {
                throw new ArgumentException($"Expected type {typeof(RfwsSection)} but instead type is {group.GetType()}",
                    nameof(group));
            }
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
                result = RfwsParserUtilities.ParseSiNotationDouble(textValue);
            else if (t == typeof(string))
                result = textValue;
            else
                throw new NotSupportedException();

            return (T)result;
        }
        protected sealed override object ReadValueFromInput(PropertyGroup group, FieldInfo field)
        {
            RfwsSection rfwsGroup = (RfwsSection)group;
            RfwsPropertyAttribute attr = field.GetCustomAttribute<RfwsPropertyAttribute>();
            return rfwsGroup.SectionRoot.ReadKeyValue(attr.Key);
        }

    }

    /// <summary>
    /// Contains various utility functions for parsing data from the RFWS file
    /// </summary>
    public static class RfwsParserUtilities
    {
        /// <summary>
        /// Finds descendant nodes named <i>section</i> where the <i>name</i> attribute matches <paramref name="sectionName"/>.
        /// </summary>
        /// <param name="root">Specifies the node whose descendants should be searched.</param>
        /// <param name="sectionName">Specifies the value of the name attribute of the section.</param>
        /// <param name="regexMatch">Specifies whether <paramref name="sectionName"/> should be used a regular expression matching string.</param>
        /// <returns></returns>
        public static IEnumerable<XElement> FindSections(this XElement root, string sectionName, bool regexMatch = false)
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
        /// <summary>
        /// Finds descendant nodes named <i>section</i> where the <i>name</i> attribute matches the <see cref="RfwsSectionAttribute.sectionName"/>
        /// value for the <see cref="RfwsSection"/> type defined by <paramref name="sectionType"/>.
        /// </summary>
        /// <param name="root">Specifies the node whose descendants should be searched.</param>
        /// <param name="sectionType">Specifies the type of <see cref="RfwsSection"/> to find in the XML data.</param>
        public static IEnumerable<XElement> FindSections(this XElement root, Type sectionType)
        {
            (string sectionName, string version, bool regexMatch) = sectionType.GetCustomAttribute<RfwsSectionAttribute>();

            return root.FindSections(sectionName, regexMatch);
        }
        /// <summary>
        /// Reads a value from the first descendant node named <i>key</i> whose <i>name</i> attribute matches <paramref name="keyName"/>.
        /// </summary>
        /// <param name="element">Specifies the node whose descendants should be searched.</param>
        /// <param name="keyName">Specifies the value of the name attribute of the key.</param>
        /// <exception cref="KeyNotFoundException">Thrown if <paramref name="keyName"/> is not found.</exception>
        /// <returns></returns>
        public static string ReadKeyValue(this XElement element, string keyName)
        {
            string result = (from child in element.Descendants("key")
                             where (string)child.Attribute("name") == keyName
                             select child.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(result))
                throw new KeyNotFoundException($"Property named \"{keyName}\" not found.");
            else
                return result;
        }

        private static readonly Dictionary<char, double> SiDictionary = new Dictionary<char, double>
        {
            ['p'] = 1e-9,
            ['u'] = 1e-6,
            ['m'] = 1e-3,
            ['k'] = 1e3,
            ['M'] = 1e6,
            ['G'] = 1e9
        };

        /// <summary>
        /// Parses <paramref name="valueToConvert"/> and attempts to convert to a double. Regular doubles will be parsed as normal. If 
        /// the standard parsing operation fails, the string will be evaluated for an SI suffix and translated to the appropriate
        /// double value.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if <paramref name="valueToConvert"/> does not match any known SI suffixes or is malformed.</exception>
        /// <param name="valueToConvert">Specifies the string to attempt to parse.</param>
        public static double ParseSiNotationDouble(string valueToConvert)
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
        /// <summary>
        /// Simple extension function to generically convert a string to an enum.
        /// </summary>
        /// <typeparam name="T">Specifies the enum type to conver <paramref name="value"/> to.</typeparam>
        /// <param name="value">Specifies the string to parse to the desired enum type.</param>
        /// <returns></returns>
        private static T StringToEnum<T>(string value) where T : Enum
        {
            // Strip whitespace
            value = value.Replace(" ", string.Empty);
            return (T)Enum.Parse(typeof(T), value, true);
        }
        /// <summary>
        /// Converts a linear ratio value string from Volts to decibels.
        /// </summary>
        /// <param name="value">Specifies the string value to be parsed and converted.</param>
        /// <returns></returns>
        public static double ValueTodB(string value)
        {
            double scalingFactor = double.Parse(value);
            return 20 * Math.Log(scalingFactor);
        }
    }

}
