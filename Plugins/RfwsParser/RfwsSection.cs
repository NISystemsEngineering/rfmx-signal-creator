using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.RFmx.InstrMX;
using System.Xml.Linq;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{
    public abstract class RfwsSection<T> where T : ISignalConfiguration
    {
        public string SelectorString { get; protected set; }
        public T Signal { get; protected set; }
        public XElement SectionRoot { get; protected set; }
        public XElement DocumentRoot { get; protected set; }

        public RfwsSection(XElement documentRoot, XElement section, T signal, string selectorString)
        {
            Signal = signal;
            SelectorString = selectorString;
            SectionRoot = section;
            DocumentRoot = documentRoot;
        }

        public void Deconstruct(out XElement section, out T signal, out string selectorString)
        {
            section = SectionRoot;
            signal = Signal;
            selectorString = SelectorString;
        }
    }
}
