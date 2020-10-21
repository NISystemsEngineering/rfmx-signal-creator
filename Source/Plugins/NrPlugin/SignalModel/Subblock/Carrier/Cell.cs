using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using Serialization;

    internal class Cell
    {
        [RfwsDeserializableKey("Cell ID", 3)]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.CellID)]
        public int? CellId;
        [RfwsDeserializableKey("Bandwidth (Hz)", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.ComponentCarrierBandwidth)]
        public double? Bandwidth;
        [RfwsDeserializableKey("Frequency Range", 3)]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.FrequencyRange, RfmxNrSelectorStringType.Subblock)]
        public RFmxNRMXFrequencyRange? FrequencyRange;
        [RfwsDeserializableKey("Reference Grid Alignment Mode", 3)]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.ReferenceGridAlignmentMode, RfmxNrSelectorStringType.None)]
        public RFmxNRMXReferenceGridAlignmentMode? RefGridAlignmentMode;
        [RfwsDeserializableKey("Reference Grid Subcarrier Spacing", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.ReferenceGridSubcarrierSpacing)]
        public double? ReferenceGridSubcarrierSpacing;
        [RfwsDeserializableKey("Reference Grid Start", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.ReferenceGridStart)]
        public int? ReferenceGridStart;
        // This key is only located here in RFmx 19.1; it is moved to the CarrierSet section in 20.0
        [RfwsDeserializableKey("Auto Increment Cell ID Enabled", 3, RfwsVersionMode.SpecificVersions)]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.AutoIncrementCellIDEnabled, RfmxNrSelectorStringType.None)]
        public bool? AutoIncrementCellId_19_1;

    }
}
