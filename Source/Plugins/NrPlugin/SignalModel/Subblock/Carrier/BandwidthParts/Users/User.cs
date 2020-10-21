using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using Serialization;

    internal class User
    {
        [RfwsDeserializableKey("UE Index", 1)]
        public int? UeIndex;
        [RfwsDeserializableKey("nRNTI", 1), RFmxNrSerializableProperty(RFmxNRMXPropertyId.Rnti)]
        public int? Rnti;

        [RfwsDeserializableSection("PDSCH Settings", version = "3"), RFmxSerializableSection]
        public PdschSettings PdschConfiguration;
        [RfwsDeserializableSection("PUSCH Settings", version = "1"), RFmxSerializableSection]
        public PuschSettings PuschConfiguration;
    }
}
