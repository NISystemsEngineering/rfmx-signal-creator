using System;
using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.Utilities.WaveformParsing
{
    /// <summary>
    /// Adds additional mapping functionality on the base to support subsections in the RFWS file
    /// </summary>
    public abstract class RfwsMapper<T> : MapperCore<T> where T : ISignalConfiguration
    {
        public RfwsMapper(T signal)
            : base(signal)
        {

        }
        public override void Map(PropertyGroup group)
        {
            if (group is RfwsSection rfwsGroup)
            {
                base.Map(rfwsGroup);
                foreach (RfwsSection subsection in rfwsGroup.SubSections)
                {
                    Map(subsection);
                }
            }
            else
            {
                throw new ArgumentException($"Expected type {typeof(RfwsSection)} but instead type is {group.GetType()}",
                    nameof(group));
            }
        }
    }
}
