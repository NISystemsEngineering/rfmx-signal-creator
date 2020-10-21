using NationalInstruments.RFmx.NRMX;
using System.Collections.Generic;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using Serialization;

    internal class Subblock 
    {
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.SubblockFrequencyDefinition)]
        public RFmxNRMXSubblockFrequencyDefinition SubblockFrequencyDefinition { get; } = RFmxNRMXSubblockFrequencyDefinition.Absolute;

        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.NumberOfComponentCarriers)]
        public int NumberOfComponentCarriers;
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.CenterFrequency)]
        public double? CarrierSubblockOffset;

        [RFmxSerializableSection(SelectorStrings.Carrier)]
        public List<Carrier> ComponentCarriers;
    }
}
