using NationalInstruments.RFmx.NRMX;
using System.Collections.Generic;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    public class Subblock 
    {
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.SubblockFrequencyDefinition)]
        public RFmxNRMXSubblockFrequencyDefinition SubblockFrequencyDefinition { get; } = RFmxNRMXSubblockFrequencyDefinition.Absolute;

        [RFmxNrMappableProperty(RFmxNRMXPropertyId.NumberOfComponentCarriers)]
        public int NumberOfComponentCarriers;
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.CenterFrequency)]
        public double? CarrierSubblockOffset;

        [RFmxMappableSection(SelectorStrings.Carrier)]
        public List<Carrier> ComponentCarriers;
    }
}
