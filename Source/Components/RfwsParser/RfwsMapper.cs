using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.Utilities.WaveformParsing
{
    public abstract class RfwsMapper<T> : MapperCore<T> where T : ISignalConfiguration
    {
        public RfwsMapper(T signal)
            : base(signal)
        {

        }
        public override void Map(PropertyGroup group)
        {
            base.Map(group);
            if (group is RfwsSection rfwsGroup)
            {
                foreach (RfwsSection subsection in rfwsGroup.SubSections)
                {
                    Map(subsection);
                }
            }
        }
    }
}
