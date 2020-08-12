using System;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using System.Collections.Generic;
using NationalInstruments.RFmx.InstrMX;
using Serilog;

namespace NationalInstruments.Utilities.WaveformParsing
{
    /// <summary>
    /// Represents a section (tag "section") contained within an RFWS file and the RFmx signal and selector string
    /// required to configure it.
    /// </summary>
    public abstract class RfwsSection : PropertyGroup
    {
        public const string KeyVersion = "version";

        public RfwsPropertyAttribute GetAttribute(FieldInfo field)
            => field.GetCustomAttribute<RfwsPropertyAttribute>();

        public RfwsSectionAttribute SectionAttribute
            => GetType().GetCustomAttribute<RfwsSectionAttribute>();

        private List<RfwsSection> _subSections;

        public virtual IEnumerable<RfwsSection> SubSections => _subSections;


        /// <summary>Specifies the root element represented by this section.</summary>
        public XElement SectionRoot { get; protected set; }
        /// <summary>Specifies the version of the section loaded at runtime from the "version" attribute of the section.</summary>
        public float Version { get => float.Parse(SectionRoot.Attribute(KeyVersion).Value); }

        protected RfwsSection(XElement propertySection)
            : base(string.Empty)
        {
            SectionRoot = propertySection;
            InitSubsections();
        }

        protected RfwsSection(XElement propertySection, RfwsSection parentGroup)
            : base(parentGroup.SelectorString)
        {
            SectionRoot = propertySection;
            InitSubsections();
        }
        private void InitSubsections()
        {
            _subSections = new List<RfwsSection>();
            var subSectionFields = GetSubSectionFields();

            foreach ((FieldInfo field, RfwsSectionAttribute attr) in subSectionFields)
            {
                XElement childSection;

                // Determine if 
                if (attr is RfwsSectionListAttribute sectionAttr
                    && sectionAttr.SameSectionAsParent == true)
                {
                    childSection = SectionRoot;
                }
                else
                {
                    try
                    {
                        childSection = RfwsParserUtilities.FindSections(SectionRoot, attr.sectionName).First();
                    }
                    catch (InvalidOperationException)
                    {
                        Log.Error("Expected to find section named {SectionName} with Regex match {Status} but no match was found", 
                            attr.sectionName, attr.regExMatch ? "enabled" : "disabled");
                        continue;
                    }
                }
                try
                {
                    RfwsSection group = (RfwsSection)Activator.CreateInstance(field.FieldType, childSection, this);
                    _subSections.Add(group);
                    field.SetValue(this, group);
                }
                catch (MissingMethodException ex)
                {
                    Log.Debug(ex, "Failed to create instance of type {Type}. Invalid constructor specified for class.", field.FieldType);
                }
            }

        }

        private IEnumerable<(FieldInfo field, RfwsSectionAttribute attr)> GetSubSectionFields()
        {
            return from field in GetType().GetFields()
                   let type = field.FieldType
                   where type.IsSubclassOf(typeof(RfwsSection))
                   where field.IsDefined(typeof(RfwsSectionAttribute))
                   let attr = field.GetCustomAttribute<RfwsSectionAttribute>()
                   select (field, attr);
        }
    }
}
