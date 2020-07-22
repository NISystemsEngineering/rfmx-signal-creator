using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.RFmx.InstrMX;
using System.Xml.Linq;
using System.Reflection;
using System.IO;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{

    public abstract class RfwsSection<T> where T : ISignalConfiguration
    {
        public const string KeyVersion = "version";

        public string SelectorString { get; protected set; }
        public T Signal { get; protected set; }
        public XElement SectionRoot { get; protected set; }
        public XElement DocumentRoot { get; protected set; }

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
