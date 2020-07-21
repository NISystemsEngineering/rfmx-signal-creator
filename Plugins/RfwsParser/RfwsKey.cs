using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{
    public abstract class RfwsKey<T>
    {
        private T value;

        public int RfmxPropertyId;
        /// <summary>
        /// Optional; use if the string value from the RFWS file cannot be directly mapped to the RFmx value. For example,
        /// a value that must be translated to a specific RFmx enum. If this delegate is not specified (i.e. null) the value
        /// from the XML file will be parsed directly to the type specified by <see cref="RfmxType"/>.
        /// </summary>
        public Func<string, T> CustomMap;

        public bool HasValue { get; private set; } = false;
        public T Value
        {
            get
            {
                if (!HasValue) throw new InvalidOperationException("Value has not been set");
                else return value;
            }
            set
            {
                this.value = value;
                HasValue = true;
            }
        }
    }
}
