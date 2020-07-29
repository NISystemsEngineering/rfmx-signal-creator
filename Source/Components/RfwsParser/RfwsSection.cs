using System.Xml.Linq;
using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.Utilities.WaveformParsing
{
    /// <summary>
    /// Represents a section (tag "section") contained within an RFWS file and the RFmx signal and selector string
    /// required to configure it.
    /// </summary>
    /// <typeparam name="T">Specifies the RFmx signal type that this section will configure.</typeparam>
    public abstract class RfwsSection
    {
        public const string KeyVersion = "version";
        protected string parentSelectorString = "";

        /// <summary>Specifies the RFmx selector string needed to configure this section of the file.</summary>
        public virtual string SelectorString
        {
            get => parentSelectorString;
        }
        /// <summary>Specifies the root element represented by this section.</summary>
        public XElement SectionRoot { get; protected set; }
        /// <summary>Specifies the root of the entire XML document.</summary>
        public XElement DocumentRoot { get; protected set; }
        /// <summary>Specifies the version of the section loaded at runtime from the "version" attribute of the section.</summary>
        public float Version { get => float.Parse(SectionRoot.Attribute(KeyVersion).Value); }

        /// <summary>
        /// An optional override when a section needs to specifically configure the RFmx session with information included in that section
        /// that does not directly map to an RFmx property. The <see cref="RfmxMapper{T}"/> object will call this method <b>prior to appying
        /// any keys to the signal.</b>
        /// </summary>
        /// <param name="signal">Specifies the RFmx signal that will be configured.</param>
        public virtual void ConfigureRFmxSignal(ISignalConfiguration signal) { }

        protected RfwsSection(XElement documentRoot, XElement section, string selectorString)
        {
            parentSelectorString = selectorString;
            SectionRoot = section;
            DocumentRoot = documentRoot;
        }

        protected RfwsSection(XElement childSection, RfwsSection parentSection)
        {
            parentSelectorString = parentSection.SelectorString;
            SectionRoot = childSection;
            DocumentRoot = parentSection.DocumentRoot;
        }
    }
}
