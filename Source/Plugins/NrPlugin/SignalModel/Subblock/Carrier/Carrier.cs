using System.Collections.Generic;
using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using Serialization;

    [RfwsDeserializableSection("CarrierDefinition", version = "1")]
    internal class Carrier 
    {
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.ComponentCarrierFrequency)]
        public double? CarrierFrequencyOffset;

        [RfwsDeserializableKey("Bandwidth Part Count", 1)]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.NumberOfBandwidthParts)]
        public int? BandwidthPartCount;
        // Current version (20.0) is 5, but properties below also work with 19.1 (version 3)

        [RfwsDeserializableSection("Cell Settings", version = "5")]
        public Cell Cell;
        [RfwsDeserializableSection("Output Settings", version = "3")]
        public OutputSettings Output;
        [RfwsDeserializableSection(@"Ssb Settings", version = "4")]
        public SsbSettings Ssb;
        [RfwsDeserializableSection(@"Bandwidth Part Settings \d+", version = "3", regExMatch = true)]
        [RFmxSerializableSection(SelectorStrings.BandwidthPart)]
        public List<BandwidthPartSettings> BandwidthParts;

        public Carrier ShallowCopy()
        {
            return (Carrier)MemberwiseClone();
        }
    }

}
