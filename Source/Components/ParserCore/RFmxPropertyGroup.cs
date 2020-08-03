using System.Reflection;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.Utilities.WaveformParsing
{
    /// <summary>
    /// Represents a section (tag "section") contained within an RFWS file and the RFmx signal and selector string
    /// required to configure it.
    /// </summary>
    public abstract class RFmxPropertyGroup
    {
        protected string parentSelectorString = "";

        /// <summary>Specifies the RFmx selector string needed to configure this section of the file.</summary>
        public virtual string SelectorString
        {
            get => parentSelectorString;
        }

        /// <summary>
        /// An optional override when a section needs to specifically configure the RFmx session with information included in that section
        /// that does not directly map to an RFmx property. The <see cref="RfmxMapper{T}"/> object will call this method <b>prior to appying
        /// any keys to the signal.</b>
        /// </summary>
        /// <param name="signal">Specifies the RFmx signal that will be configured.</param>
        public virtual void ConfigureRFmxSignal(ISignalConfiguration signal) { }

        protected RFmxPropertyGroup(string selectorString)
        {
            parentSelectorString = selectorString;
        }

        public IEnumerable<FieldValuePair> FetchMappedFields()
        {
            var fields = from field in GetType().GetFields() // Get all configured fields for the ipnut type
                       where field.FieldType.IsSubclassOf(typeof(RFmxPropertyMap<>))
                       let fieldPair = new FieldValuePair(field, this) // Save the value of the field (aka the map defined at edit time)
                       select fieldPair; // Return the key map pair
            return fields;
        }
    }
}
