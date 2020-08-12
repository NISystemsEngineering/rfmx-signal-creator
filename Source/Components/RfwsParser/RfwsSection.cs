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
        #region Fields & Properties
        public const string KeyVersion = "version";
        private List<RfwsSection> _subSections;

        /// <summary>Specifies the root element represented by this section.</summary>
        public XElement SectionRoot { get; protected set; }
        /// <summary>Specifies the version of the section loaded at runtime from the "version" attribute of the section.</summary>
        public float Version => float.Parse(SectionRoot.Attribute(KeyVersion).Value);
        /// <summary>Returns all of the public fields of type <see cref="RfwsSection"/> contained within this class.</summary>
        public virtual IEnumerable<RfwsSection> SubSections => _subSections;
        /// <summary>Fetches the section attribute associated with this class.</summary>
        public RfwsSectionAttribute SectionAttribute => GetType().GetCustomAttribute<RfwsSectionAttribute>();
        #endregion

        // Used to initialize root sections
        protected RfwsSection(XElement propertySection)
            : base(string.Empty)
        {
            SectionRoot = propertySection;
            InitSubsections();
        }
        // Used to initialize subsections
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

                #region Long Comment
                // Determine if this child field is of the special subsection type that impelments a list
                // If so, we may need to handle its section root in a more specific way. See the two examples below
                // showcase the difference.
                //
                // -------------------------------------------------
                // SameSectionAsParent = true
                // -------------------------------------------------
                // <section name="CarrierDefinition" version="1">
                //      <section name="Bandwidth Part Settings 0" version="3">      <- Subsections are directly children of the root element
                //          <key name="Subcarrier Spacing (Hz)">60k</key>
                //          ...
                //      </section>
                //      <section name="Bandwidth Part Settings 1" version="3">
                //          <key name="Subcarrier Spacing (Hz)">60k</key>
                //          ...
                //      </section>
                // -------------------------------------------------
                // SameSectionAsParent = false
                // -------------------------------------------------
                // <section name="CarrierManager" version="3">                      <- Higher level section defines list section
                //      <key name="count">2</key>
                //      <section name="0" version="1">                           
                //              <section name="Carrier" version="3">                <- Subsections are a root of the higher level section
                //                  ...
                //              </section>
                //      </section>
                //      <section name = "1" version="1">
                //              <section name="Carrier" version="3">
                //                  ...
                // -------------------------------------------------
                #endregion

                if (attr is RfwsSectionListAttribute sectionAttr
                    && sectionAttr.SameSectionAsParent == true)
                {
                    childSection = SectionRoot;
                }
                else
                {
                    // Find the child section from the root node
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
