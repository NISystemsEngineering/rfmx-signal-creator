using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using SignalCreator.RfwsParser;

    public class CoresetSettings
    {
        private int? parsedRegBundleSize;
        private int? parsedInterleaverSize;
        private int? parsedShiftIndex;

        [RFmxNrMappableProperty(RFmxNRMXPropertyId.CoresetNumberOfResourceBlockClusters)]
        public int? NumberOfRbClusters => CoresetRbClusters?.Count;

        [RfwsParseableKey("RB Allocation", 1, ConverterType = typeof(RbClusterConverter<CoresetRbCluster>))]
        [RFmxMappableSection(SelectorStrings.CoresetCluster)]
        public List<CoresetRbCluster> CoresetRbClusters;

        #region RFmx Properties

        [RfwsParseableKey("Coreset Num Symbols", 1), RFmxNrMappableProperty(RFmxNRMXPropertyId.CoresetNumberOfSymbols)]
        public int? NumberOfSymbols;
        [RfwsParseableKey("Symbol Offset", 1), RFmxNrMappableProperty(RFmxNRMXPropertyId.CoresetSymbolOffset)]
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

        [RfwsParseableKey("Precoder Granularity", 1, ConverterType = typeof(GanularityLookupConverter))]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.CoresetPrecodingGranularity)]
        public RFmxNRMXCoresetPrecodingGranularity? PrecoderGranularity;
        [RfwsParseableKey("CCE to REG Mapping Type", 1), RFmxNrMappableProperty(RFmxNRMXPropertyId.CoresetCceToRegMappingType)]
        public RFmxNRMXCoresetCceToRegMappingType? CceToRegMappingType;

        // RFmx Waveform Creator disables (grays out) the Reg Bundle Size, Interleaver Size, and Shift Index property when mapping is set to non-interleaved.
        // However, the file still has a default value when not set. This value is likely incorrect so we avoid setting this for the user (hence the following get/set properties).

        [RfwsParseableKey("REG Bundle Size", 1), RFmxNrMappableProperty(RFmxNRMXPropertyId.CoresetRegBundleSize)]
        public int? RegBundleSize
        {
            get => CceToRegMappingType.Value == RFmxNRMXCoresetCceToRegMappingType.NonInterleaved ? null : parsedRegBundleSize;
            set => parsedRegBundleSize = value;
        }
        [RfwsParseableKey("Interleaver Size", 1), RFmxNrMappableProperty(RFmxNRMXPropertyId.CoresetInterleaverSize)]
        public int? InterleaverSize
        {
            get => CceToRegMappingType.Value == RFmxNRMXCoresetCceToRegMappingType.NonInterleaved ? null : parsedInterleaverSize;
            set => parsedInterleaverSize = value;
        }
        [RfwsParseableKey("Shift Index", 1), RFmxNrMappableProperty(RFmxNRMXPropertyId.CoresetShiftIndex)]
        public int? ShiftIndex
        {
            get => CceToRegMappingType.Value == RFmxNRMXCoresetCceToRegMappingType.NonInterleaved ? null : parsedShiftIndex;
            set => parsedShiftIndex = value;
        }
        [RfwsParseableKey("PDCCH Slot Count", 1), RFmxNrMappableProperty(RFmxNRMXPropertyId.NumberOfPdcchConfigurations)]
        public int? NumberOfPdcchConfigs;
        #endregion
        #region Properties Not Currently Implemented as of RFmx 20.0
        // Does not appear to be supported in RFmx 20.0
        [RfwsParseableKey("PDCCH UE Count", 1)]
        public int? PdcchUeCount;
        // Not implemented: DMRS Scrambling ID/Mode. Do not appear to be implemented in RFmx NR 20.0
        [RfwsParseableKey("DMRS Scrambling ID", 1)]
        public int? DmrsScramblingId;
        [RfwsParseableKey("DMRS Scrambling ID Mode", 1)]
        public string DmrsScramblingIdMode;

        #endregion

        [RfwsSection(@"PDCCH Slot Settings \d+", version = "3", regExMatch = true)]
        [RFmxMappableSection(SelectorStrings.Pddch)]
        public List<Pdcch> PdcchConfigs;
    }
}
