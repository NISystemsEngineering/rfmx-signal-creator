using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Serilog;

namespace NationalInstruments.Utilities.SignalCreator.RfwsParser
{
    public class RfwsParserV2 : ParserCorev2
    {
        public object Parse(Type type, XElement section)
        {
            return ParseValue(type, section);
        }
        public T Parse<T>(XElement section)
        {
            return (T)Parse(typeof(T), section);
        }

        public override object ReadValue(Type t, object valueToParse, ParseableAttribute attr)
        {
            XElement section = (XElement)valueToParse;

            switch (attr)
            {
                case RfwsParseableKeyAttribute keyAttr:
                    object value = section.ReadKeyValue(keyAttr.Key);
                    if (t == typeof(double))
                    {
                        return RfwsParserUtilities.ParseSiNotationDouble((string)value);
                    }
                    else return value;
                case RfwsSectionAttribute sectionAttr:
                    if (t.IsArray || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        return section.FindSections(sectionAttr.sectionName, sectionAttr.regExMatch);
                    }
                    else return section.FindSections(sectionAttr.sectionName, sectionAttr.regExMatch).First();
                default: return null;
            }
        }
        public override bool SelectValidAttribute(IEnumerable<ParseableAttribute> attributes, object valueToParse, out ParseableAttribute validAttr)
        {
            if (attributes.First() is RfwsParseableKeyAttribute)
            {
                XElement section = (XElement)valueToParse;
                float sectionVersion = section.GetSectionVersion();
                validAttr = attributes.Cast<RfwsParseableKeyAttribute>().Where(attr => attr.IsSupported(sectionVersion)).FirstOrDefault();
                return validAttr != null;
            }
            else return base.SelectValidAttribute(attributes, valueToParse, out validAttr);
        }
    }
    public static class RfwsParserExtensions
    {
        public static T Parse<T>(this XElement element)
        {
            RfwsParserV2 parser = new RfwsParserV2();
            return parser.Parse<T>(element);
        }
    }
}
