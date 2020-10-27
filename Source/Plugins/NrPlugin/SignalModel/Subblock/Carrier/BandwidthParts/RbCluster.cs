using System.Linq;
using System.Collections.Generic;
using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using Serialization.Converters;

    /// <summary>
    /// Converts a string of RB cluster allocations to a list of paired offset and number of resource blocks.<para></para>
    /// <example>For example,"0:2,3,5,10:last" becomes [ (0,3), (3,1), (5,1), (10,-1) ].</example>
    /// </summary>
    /// <typeparam name="T">Specifies the specific type of RB cluster for different sections of the NR signal.</typeparam>
    internal class RbClusterConverter<T> : ValueConverter<List<T>> where T : RbCluster, new()
    {
        protected override List<T> Convert(object value)
        {
            string textValue = (string)value;
            var result = NrParsingUtilities.ParseRbAllocationString(textValue);

            var rbClusters = result.Select(el => new T { NumRbs = el.numRbs, RbOffset = el.offset });

            return rbClusters.ToList();
        }
    }
    /// <summary>
    /// Represents a resource block cluster. Implemented as an abstract class so that child classes
    /// can define the specific attributes for the elements to map them appropriately to the specific
    /// NR section intended.
    /// </summary>
    internal abstract class RbCluster
    {
        public abstract int? RbOffset { get; set; }
        public abstract int? NumRbs { get; set; }
    }

    internal class PdschRbCluster : RbCluster
    {
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschResourceBlockOffset)]
        public override int? RbOffset { get; set; }
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschNumberOfResourceBlocks)]
        public override int? NumRbs { get; set; }
    }
    internal class PuschRbCluster : RbCluster
    {
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschResourceBlockOffset)]
        public override int? RbOffset { get; set; }
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschNumberOfResourceBlocks)]
        public override int? NumRbs { get; set; }
    }
    internal class CoresetRbCluster : RbCluster
    {
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.CoresetResourceBlockOffset)]
        public override int? RbOffset { get; set; }
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.CoresetNumberOfResourceBlocks)]
        public override int? NumRbs { get; set; }
    }
}
