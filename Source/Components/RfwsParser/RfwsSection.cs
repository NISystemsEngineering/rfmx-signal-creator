using System;
using System.Xml.Linq;
using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{
    /// <summary>
    /// Represents a section (tag "section") contained within an RFWS file and the RFmx signal and selector string
    /// required to configure it.
    /// </summary>
    /// <typeparam name="T">Specifies the RFmx signal type that this section will configure.</typeparam>
    public abstract class RfwsSection<T> where T : ISignalConfiguration
    {
        public const string KeyVersion = "version";

        /// <summary>Specifies the RFmx selector string needed to configure this section of the file.</summary>
        public string SelectorString { get; protected set; }
        public virtual string SelectorString2 { get; }
        /// <summary>Specifies the RFmx signal that will be configured for this section.</summary>
        public T Signal { get; protected set; }
        /// <summary>Specifies the root element represented by this section.</summary>
        public XElement SectionRoot { get; protected set; }
        /// <summary>Specifies the root of the entire XML document.</summary>
        public XElement DocumentRoot { get; protected set; }

        public virtual void ConfigureRFmxSignal<T>(T signal) { }
        public float Version { get; }

        public RfwsSection(XElement documentRoot, XElement section, T signal, string selectorString)
        {
            Signal = signal;
            SelectorString = selectorString;
            SectionRoot = section;
            DocumentRoot = documentRoot;

            Version = float.Parse(SectionRoot.Attribute(KeyVersion).Value);
        }
        public RfwsSection(XElement childSection, RfwsSection<T> parentSection)
        {
            Signal = parentSection.Signal;
            SelectorString = parentSection.SelectorString;
            SectionRoot = childSection;
            DocumentRoot = parentSection.DocumentRoot;

            Version = float.Parse(SectionRoot.Attribute(KeyVersion).Value);
        }

        public void Deconstruct(out T signal, out string selectorString)
        {
            signal = Signal;
            selectorString = SelectorString;
        }
    }
}
