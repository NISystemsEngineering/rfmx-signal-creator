using System;
using System.Xml.Linq;
using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using SignalCreator.RfwsParser;
    using System.Collections.Generic;

    [RfwsSection("CarrierDefinition", version = "1")]
    public class Carrier 
    {
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.ComponentCarrierFrequency)]
        public double? CarrierFrequencyOffset;

        [RfwsParseableKey("Bandwidth Part Count", 1)]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.NumberOfBandwidthParts)]
        public int? BandwidthPartCount;
        // Current version (20.0) is 5, but properties below also work with 19.1 (version 3)

        [RfwsSection("Cell Settings", version = "5")]
        public Cell Cell;
        [RfwsSection("Output Settings", version = "3")]
        public OutputSettings Output;
        [RfwsSection(@"Ssb Settings", version = "4")]
        public SsbSettings Ssb;
        [RfwsSection(@"Bandwidth Part Settings \d+", version = "3", regExMatch = true)]
        [RFmxMappableSection(SelectorStrings.BandwidthPart)]
        public List<BandwidthPartSettings> BandwidthParts;

        public Carrier ShallowCopy()
        {
            return (Carrier)MemberwiseClone();
        }
    }

}
