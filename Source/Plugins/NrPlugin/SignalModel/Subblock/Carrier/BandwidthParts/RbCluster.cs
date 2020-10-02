using System.Collections.Generic;
using System.Xml.Linq;
using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using SignalCreator.RfwsParser;
    using System.Collections;

    public class RbClusterConverter<T> : ValueConverter<List<T>> where T : RbCluster, new()
    {
        protected override List<T> Convert(object value)
        {
            string textValue = (string)value;
            var result = NrParsingUtilities.ParseRbAllocationString(textValue);

            List<T> pairs = new List<T>(result.Count);
            foreach(var (offset, numRbs) in result)
            {
                T rb = new T
                {
                    NumRbs = numRbs,
                    RbOffset = offset
                };

                pairs.Add(rb);
            }
            return pairs;
        }
    }
    public abstract class RbCluster
    {
        public abstract int? RbOffset { get; set; }
        public abstract int? NumRbs { get; set; }
    }

    public class PdschRbCluster : RbCluster
    {
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschResourceBlockOffset)]
        public override int? RbOffset { get; set; }
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschNumberOfResourceBlocks)]
        public override int? NumRbs { get; set; }
    }
    public class PuschRbCluster : RbCluster
    {
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschResourceBlockOffset)]
        public override int? RbOffset { get; set; }
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschNumberOfResourceBlocks)]
        public override int? NumRbs { get; set; }
    }
    public class CoresetRbCluster : RbCluster
    {
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.CoresetResourceBlockOffset)]
        public override int? RbOffset { get; set; }
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.CoresetNumberOfResourceBlocks)]
        public override int? NumRbs { get; set; }
    }
}
