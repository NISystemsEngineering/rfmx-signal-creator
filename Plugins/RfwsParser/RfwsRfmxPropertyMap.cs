using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{
    /// <summary>
    /// Defines the types of values that the RFmx Set Property functions accept
    /// </summary>
    public enum RmfxPropertyTypes
    {
        Bool,
        Double,
        Int,
        String,
        Sbyte
    }
    public class RfwsRfmxPropertyMap
    {
        public int RfmxPropertyId;
        public RmfxPropertyTypes RfmxType;
        /// <summary>
        /// Optional; use if the string value from the RFWS file cannot be directly mapped to the RFmx value. For example,
        /// a value that must be translated to a specific RFmx enum. If this delegate is not specified (i.e. null) the value
        /// from the XML file will be parsed directly to the type specified by <see cref="RfmxType"/>.
        /// </summary>
        public Func<string, object> CustomMap;
    }
}
