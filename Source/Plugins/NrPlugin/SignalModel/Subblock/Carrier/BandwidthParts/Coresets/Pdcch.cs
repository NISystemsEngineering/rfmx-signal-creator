using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using Serialization;

    internal class Pdcch
    {
        [RfwsDeserializableKey("Cce Aggregation Level", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdcchCceAggregationLevel)]
        public int? CceAggregationLevel;
        [RfwsDeserializableKey("CCE Offset", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdcchCceOffset)]
        public int? CceOffset;
        [RfwsDeserializableKey("Slot Allocation", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdcchSlotAllocation)]
        public string SlotAllocation;
    }
}
