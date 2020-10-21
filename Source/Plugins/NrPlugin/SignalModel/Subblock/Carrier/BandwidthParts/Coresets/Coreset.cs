using System.Collections.Generic;
using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using Serialization;
    using Serialization.Converters;

    internal class CoresetSettings
    {
        private int? parsedRegBundleSize;
        private int? parsedInterleaverSize;
        private int? parsedShiftIndex;

        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.CoresetNumberOfResourceBlockClusters)]
        public int? NumberOfRbClusters => CoresetRbClusters?.Count;

        [RfwsDeserializableKey("RB Allocation", 1, ConverterType = typeof(RbClusterConverter<CoresetRbCluster>))]
        [RFmxSerializableSection(SelectorStrings.CoresetCluster)]
        public List<CoresetRbCluster> CoresetRbClusters;

        #region RFmx Properties

        [RfwsDeserializableKey("Coreset Num Symbols", 1), RFmxNrSerializableProperty(RFmxNRMXPropertyId.CoresetNumberOfSymbols)]
        public int? NumberOfSymbols;
        [RfwsDeserializableKey("Symbol Offset", 1), RFmxNrSerializableProperty(RFmxNRMXPropertyId.CoresetSymbolOffset)]
        public int? SymbolOffset;

        private class GanularityLookupConverter : LookupTableConverter<string, RFmxNRMXCoresetPrecodingGranularity>
        {
            protected override Dictionary<string, RFmxNRMXCoresetPrecodingGranularity> LookupTable { get; } =
                new Dictionary<string, RFmxNRMXCoresetPrecodingGranularity>
                {
                    ["Same As REG Bundle"] = RFmxNRMXCoresetPrecodingGranularity.SameAsRegBundle,
                    ["All Contiguous RBs"] = RFmxNRMXCoresetPrecodingGranularity.AllContiguousResourceBlocks
                };
        }

        [RfwsDeserializableKey("Precoder Granularity", 1, ConverterType = typeof(GanularityLookupConverter))]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.CoresetPrecodingGranularity)]
        public RFmxNRMXCoresetPrecodingGranularity? PrecoderGranularity;
        [RfwsDeserializableKey("CCE to REG Mapping Type", 1), RFmxNrSerializableProperty(RFmxNRMXPropertyId.CoresetCceToRegMappingType)]
        public RFmxNRMXCoresetCceToRegMappingType? CceToRegMappingType;

        // RFmx Waveform Creator disables (grays out) the Reg Bundle Size, Interleaver Size, and Shift Index property when mapping is set to non-interleaved.
        // However, the file still has a default value when not set. This value is likely incorrect so we avoid setting this for the user (hence the following get/set properties).

        [RfwsDeserializableKey("REG Bundle Size", 1), RFmxNrSerializableProperty(RFmxNRMXPropertyId.CoresetRegBundleSize)]
        public int? RegBundleSize
        {
            get => CceToRegMappingType.Value == RFmxNRMXCoresetCceToRegMappingType.NonInterleaved ? null : parsedRegBundleSize;
            set => parsedRegBundleSize = value;
        }
        [RfwsDeserializableKey("Interleaver Size", 1), RFmxNrSerializableProperty(RFmxNRMXPropertyId.CoresetInterleaverSize)]
        public int? InterleaverSize
        {
            get => CceToRegMappingType.Value == RFmxNRMXCoresetCceToRegMappingType.NonInterleaved ? null : parsedInterleaverSize;
            set => parsedInterleaverSize = value;
        }
        [RfwsDeserializableKey("Shift Index", 1), RFmxNrSerializableProperty(RFmxNRMXPropertyId.CoresetShiftIndex)]
        public int? ShiftIndex
        {
            get => CceToRegMappingType.Value == RFmxNRMXCoresetCceToRegMappingType.NonInterleaved ? null : parsedShiftIndex;
            set => parsedShiftIndex = value;
        }
        [RfwsDeserializableKey("PDCCH Slot Count", 1), RFmxNrSerializableProperty(RFmxNRMXPropertyId.NumberOfPdcchConfigurations)]
        public int? NumberOfPdcchConfigs;
        #endregion
        #region Properties Not Currently Implemented as of RFmx 20.0
        // Does not appear to be supported in RFmx 20.0
        [RfwsDeserializableKey("PDCCH UE Count", 1)]
        public int? PdcchUeCount;
        // Not implemented: DMRS Scrambling ID/Mode. Do not appear to be implemented in RFmx NR 20.0
        [RfwsDeserializableKey("DMRS Scrambling ID", 1)]
        public int? DmrsScramblingId;
        [RfwsDeserializableKey("DMRS Scrambling ID Mode", 1)]
        public string DmrsScramblingIdMode;

        #endregion

        [RfwsDeserializableSection(@"PDCCH Slot Settings \d+", version = "3", regExMatch = true)]
        [RFmxSerializableSection(SelectorStrings.Pddch)]
        public List<Pdcch> PdcchConfigs;
    }
}
