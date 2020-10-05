using System;
using System.Xml.Linq;
using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using SignalCreator.RfwsParser;

    public class Cell
    {
        [RfwsParseableKey("Cell ID", 3)]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.CellID)]
        public int? CellId;
        [RfwsParseableKey("Bandwidth (Hz)", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.ComponentCarrierBandwidth)]
        public double? Bandwidth;
        [RfwsParseableKey("Frequency Range", 3)]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.FrequencyRange, RfmxNrSelectorStringType.Subblock)]
        public RFmxNRMXFrequencyRange? FrequencyRange;
        [RfwsParseableKey("Reference Grid Alignment Mode", 3)]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.ReferenceGridAlignmentMode, RfmxNrSelectorStringType.None)]
        public RFmxNRMXReferenceGridAlignmentMode? RefGridAlignmentMode;
        [RfwsParseableKey("Reference Grid Subcarrier Spacing", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.ReferenceGridSubcarrierSpacing)]
        public double? ReferenceGridSubcarrierSpacing;
        [RfwsParseableKey("Reference Grid Start", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.ReferenceGridStart)]
        public int? ReferenceGridStart;
        // This key is only located here in RFmx 19.1; it is moved to the CarrierSet section in 20.0
        [RfwsParseableKey("Auto Increment Cell ID Enabled", 3, RfwsVersionMode.SpecificVersions)]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.AutoIncrementCellIDEnabled, RfmxNrSelectorStringType.None)]
        public bool? AutoIncrementCellId_19_1;

    }
}
