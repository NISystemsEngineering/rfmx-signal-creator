using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.RFmx.NRMX;
using System.Xml.Linq;
using static NationalInstruments.Utilities.WaveformParsing.Plugins.RfwsParserUtilities;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{

    //[RfwsSection]
    [RfwsSection("CarrierSet", version = "3")]
    public class CarrierSet : RfwsSection<RFmxNRMX>
    {
        public CarrierSet(XElement root, XElement section, RFmxNRMX signal, string selectorString)
            : base(root, section, signal, selectorString) { }


        [RfwsProperty("AutoIncrementCellIdEnabled", 3)]
        public NrRfwsKey<bool> AutoIncrementCellId = new NrRfwsKey<bool>
        {
            RfmxPropertyId = (int)RFmxNRMXPropertyId.AutoIncrementCellIDEnabled,
            SelectorStringType = RfmxNrSelectorStringType.None
        };

        [RfwsSection("Carrier", version = "3")]
        public class Subblock : RfwsSection<RFmxNRMX>// : NrSignalModel
        {
            public const string KeySubblockNumber = "CarrierSubblockNumber";
            public const string KeyCarrierDefinition = "CarrierDefinition";
            public const string KeyCarrierCCIndex = "CarrierCCIndex";

            static double absoluteFrequency = 1e9;

            public int CarrierDefinitionIndex { get; }

            public Subblock(XElement childSection, RfwsSection<RFmxNRMX> parentSection)
                : base(childSection, parentSection)
            {
                // Build selector string for the current subblock and CC
                int subblockIndex = int.Parse(FetchValue(SectionRoot, KeySubblockNumber));

                Signal.SetNumberOfSubblocks("", subblockIndex + 1);

                SelectorString = RFmxNRMX.BuildSubblockString(SelectorString, subblockIndex);

                Signal.SetSubblockFrequencyDefinition(SelectorString, RFmxNRMXSubblockFrequencyDefinition.Absolute);

                int ccIndex = int.Parse(FetchValue(SectionRoot, KeyCarrierCCIndex));
                Signal.ComponentCarrier.SetNumberOfComponentCarriers(SelectorString, ccIndex + 1);

                SelectorString = RFmxNRMX.BuildCarrierString(SelectorString, ccIndex);

                // Fetch the carrier definition number in order to reference the appropriate carrier in a moment
                string carrierDefinitionIndex = FetchValue(SectionRoot, KeyCarrierDefinition);
                CarrierDefinitionIndex = int.Parse(carrierDefinitionIndex);

                Console.WriteLine($"Configuring subblock {subblockIndex}, component carrier {ccIndex}");
            }

            [RfwsProperty("CarrierSubblockOffset", 3)]
            public NrRfwsKey<Double> CarrierSubblockOffset = new NrRfwsKey<double>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.CenterFrequency,
                SelectorStringType = RfmxNrSelectorStringType.Subblock,
                CustomMap = (value) => SiNotationToStandard(value) + absoluteFrequency,
            };
            [RfwsProperty("CarrierFrequencyOffset", 3)]
            public NrRfwsKey<double> CarrierFrequencyOffset = new NrRfwsKey<double>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.ComponentCarrierFrequency,
            };

        }

    }

    [RfwsSection("CarrierDefinition", version = "1")]
    public class Carrier : RfwsSection<RFmxNRMX>
    {
        public const string SectionCarrierDefinitionManager = "CarrierDefinitionManager";

        public Carrier(XElement childSection, RfwsSection<RFmxNRMX> parentSection)
            : base(childSection, parentSection)
        {
        }

        [RfwsSection("Cell Settings", version = "5")]
        public class CellSettings : RfwsSection<RFmxNRMX>
        {
            public CellSettings(XElement childSection, RfwsSection<RFmxNRMX> parentSection)
                : base(childSection, parentSection) { }

            [RfwsProperty("Cell ID", 5)]
            public NrRfwsKey<int> CellId = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.CellID,
            };
            [RfwsProperty("Bandwidth (Hz)", 5)]
            public NrRfwsKey<double> Bandwidth = new NrRfwsKey<double>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.ComponentCarrierBandwidth,
            };
            [RfwsProperty("Frequency Range", 5)]
            public NrRfwsKey<int> FrequencyRange = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.FrequencyRange,
                SelectorStringType = RfmxNrSelectorStringType.Subblock,
                CustomMap = (value) => (int)StringToEnum<RFmxNRMXFrequencyRange>(value)
            };
            [RfwsProperty("Reference Grid Alignment Mode", 5)]
            public static NrRfwsKey<int> RefGridAlignmentMode = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.ReferenceGridAlignmentMode,
                SelectorStringType = RfmxNrSelectorStringType.None,
                CustomMap = (value) => (int)StringToEnum<RFmxNRMXReferenceGridAlignmentMode>(value)
            };
            [RfwsProperty("Reference Grid Subcarrier Spacing", 5)]
            public NrRfwsKey<double> RefGridSubcarrierSpacing = new NrRfwsKey<double>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.ReferenceGridSubcarrierSpacing,
            };
            [RfwsProperty("Reference Grid Start", 5)]
            public NrRfwsKey<int> ReferenceGridStart = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.ReferenceGridStart,
            };
        }
        [RfwsSection("Output Settings", version = "3")]
        public class OutputSettings : RfwsSection<RFmxNRMX>
        {
            public OutputSettings(XElement childSection, RfwsSection<RFmxNRMX> parentSection)
                : base(childSection, parentSection) { }

            [RfwsProperty("Link Direction", 3)]
            public NrRfwsKey<int> LinkDirection = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.LinkDirection,
                CustomMap = (value) => (int)StringToEnum<RFmxNRMXLinkDirection>(value),
                SelectorStringType = RfmxNrSelectorStringType.None
            };
            [RfwsProperty("DL Ch Configuration Mode", 3)]
            public NrRfwsKey<int> DlChannelConfigMode = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.DownlinkChannelConfigurationMode,
                CustomMap = (value) =>
                    (int)StringToEnum<RFmxNRMXDownlinkChannelConfigurationMode>(value),
                SelectorStringType = RfmxNrSelectorStringType.None
            };
            [RfwsProperty("DL Test Model", 3)]
            public NrRfwsKey<int> DlTestModel = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.DownlinkTestModel,
                CustomMap = (value) =>
                {
                    value = value.Replace(".", "_");
                    return (int)StringToEnum<RFmxNRMXDownlinkTestModel>(value);
                },
                SelectorStringType = RfmxNrSelectorStringType.None
            };
            [RfwsProperty("DL Test Model Duplex Scheme", 3)]
            public NrRfwsKey<int> DlTestModelDuplexScheme = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.DownlinkTestModelDuplexScheme,
                CustomMap = (value) => (int)StringToEnum<RFmxNRMXDownlinkTestModelDuplexScheme>(value),
                SelectorStringType = RfmxNrSelectorStringType.None
            };
            // Not supporting for right now
            #region SSB Settings

            #endregion
        }
        // Bandwidth Part Section has number in title
        //[RfwsSection(@"Bandwidth Part Settings \d+", version = "3", regExMatch = true)]
        /*public class BandwidthPartSettings : RfwsSection<RFmxNRMX>
        {
            public const string KeyBandwidthPartIndex = "Bandwidth Part Index";

            public BandwidthPartSettings(XElement root, XElement section, RfwsParser<RFmxNRMX> parser, RFmxNRMX signal, string selectorString)
                : base(root, section, parser, signal, selectorString)
            {
                int bandwidthPartIndex = int.Parse(FetchValue(SectionRoot, KeyBandwidthPartIndex));
                Signal.ComponentCarrier.SetNumberOfBandwidthParts(SelectorString, bandwidthPartIndex + 1);
                SelectorString = RFmxNRMX.BuildBandwidthPartString(SelectorString, bandwidthPartIndex);
            }


            [RfwsProperty("Subcarrier Spacing (Hz)", 3)]
            public NrRfwsKey2 SubcarrierSpacing = new NrRfwsKey
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.BandwidthPartSubcarrierSpacing,
                RfmxType = RmfxPropertyTypes.Double,
            };
            [RfwsProperty("Cyclic Prefix Mode", 3)]
            public NrRfwsKey2 CyclicPrefixMode = new NrRfwsKey
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.BandwidthPartCyclicPrefixMode,
                RfmxType = RmfxPropertyTypes.Int,
                CustomMap = (value) => StringToEnum<RFmxNRMXBandwidthPartCyclicPrefixMode>(value)
            };
            [RfwsProperty("Grid Start", 3)]
            public NrRfwsKey2 GridStart = new NrRfwsKey
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.GridStart,
                RfmxType = RmfxPropertyTypes.Int
            };
            [RfwsProperty("RB Offset", 3)]
            public NrRfwsKey2 RbOffset = new NrRfwsKey
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.BandwidthPartResourceBlockOffset,
                RfmxType = RmfxPropertyTypes.Int
            };
            [RfwsProperty("Number of RBs", 3)]
            public NrRfwsKey2 NumberOfRbs = new NrRfwsKey
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.BandwidthPartNumberOfResourceBlocks,
                RfmxType = RmfxPropertyTypes.Int
            };
            [RfwsProperty("UE Count", 3)]
            public NrRfwsKey2 NumberOfUe = new NrRfwsKey
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.NumberOfUsers,
                RfmxType = RmfxPropertyTypes.Int
            };
            [RfwsProperty("Coreset Count", 3)]
            public NrRfwsKey2 NumberOfCoreset = new NrRfwsKey
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.NumberOfCoresets,
                RfmxType = RmfxPropertyTypes.Int
            };

            // Section has number in title
            [RfwsSection(@"UE Settings \d+", version = "1", regExMatch = true)]
            public class UeSettings : RfwsSection<RFmxNRMX>
            {
                public const string KeyUeIndex = "UE Index";

                public UeSettings(XElement root, XElement section, RfwsParser<RFmxNRMX> parser, RFmxNRMX signal, string selectorString)
                    : base(root, section, parser, signal, selectorString)
                {
                    int ueIndex = int.Parse(FetchValue(SectionRoot, KeyUeIndex));
                    SelectorString = RFmxNRMX.BuildUserString(SelectorString, ueIndex);
                }

                [RfwsProperty("nRNTI", 1)]
                public NrRfwsKey2 Rnti = new NrRfwsKey
                {
                    RfmxPropertyId = (int)RFmxNRMXPropertyId.Rnti,
                    RfmxType = RmfxPropertyTypes.Int
                };

                // Section has number in title
                [RfwsSection(@"PDSCH Slot Settings \d+", version = "4", regExMatch = true)]
                public class PdschSlotSettings : RfwsSection<RFmxNRMX>
                {
                    public const string KeyPdschSlotIndex = "Array Index";

                    public PdschSlotSettings(XElement root, XElement section, RfwsParser<RFmxNRMX> parser, RFmxNRMX signal, string selectorString)
                        : base(root, section, parser, signal, selectorString)
                    {
                        int pdschIndex = int.Parse(FetchValue(SectionRoot, KeyPdschSlotIndex));
                        Signal.ComponentCarrier.SetNumberOfPdschConfigurations(SelectorString, pdschIndex + 1);
                        SelectorString = RFmxNRMX.BuildPdschString(SelectorString, pdschIndex);
                    }

                    [RfwsProperty("PDSCH Present in SSB RB", 4)]
                    public NrRfwsKey2 PdschPressentInSsbRb = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschPresentInSsbResourceBlock,
                        RfmxType = RmfxPropertyTypes.Bool
                    };
                    [RfwsProperty("Slot Allocation", 4)]
                    public NrRfwsKey2 RbAllocation = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschSlotAllocation,
                        RfmxType = RmfxPropertyTypes.String
                    };
                    [RfwsProperty("Symbol Allocation", 4)]
                    public NrRfwsKey2 SymbolAllocation = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschSymbolAllocation,
                        RfmxType = RmfxPropertyTypes.String
                    };
                    [RfwsProperty("Modulation Type", 4)]
                    public NrRfwsKey2 ModulationType = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschModulationType,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPdschModulationType>(value)
                    };
                    [RfwsProperty("PDSCH Mapping Type", 4)]
                    public NrRfwsKey2 MappingType = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschMappingType,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPdschMappingType>(value)
                    };
                    [RfwsProperty("DMRS Duration", 4)]
                    public NrRfwsKey2 DmrsDuration = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsDuration,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPdschDmrsDuration>(value)
                    };
                    [RfwsProperty("DMRS Configuration", 4)]
                    public NrRfwsKey2 DmrsConfiguration = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsConfigurationType,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPdschDmrsConfigurationType>(value)
                    };
                    [RfwsProperty("DMRS Power Mode", 4)]
                    public NrRfwsKey2 DmrsPowerMode = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsPowerMode,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPdschDmrsPowerMode>(value)
                    };
                    [RfwsProperty("DMRS Scaling Factor", 4)]
                    public NrRfwsKey2 DmrsPower = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsPower,
                        RfmxType = RmfxPropertyTypes.Double,
                        CustomMap = (value) => ValueTodB(value)
                    };
                    [RfwsProperty("DMRS Additional Positions", 4)]
                    public NrRfwsKey2 AdditionalPositions = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsAdditionalPositions,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("DMRS Type A Position", 4)]
                    public NrRfwsKey2 TypeAPosition = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsTypeAPosition,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("DMRS Scrambling ID", 4)]
                    public NrRfwsKey2 ScramblingId = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsScramblingID,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("DMRS Scrambling ID Mode", 4)]
                    public NrRfwsKey2 ScramblingMode = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsScramblingIDMode,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPdschDmrsScramblingIDMode>(value)
                    };
                    [RfwsProperty("Number of CDM Groups", 4)]
                    public NrRfwsKey2 CdmGroups = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsNumberOfCdmGroups,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("nSCID", 4)]
                    public NrRfwsKey2 Nscid = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsnScid,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("Dmrs Release Version", 6)]
                    public NrRfwsKey2 ReleaseVersion = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsReleaseVersion,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) =>
                        {
                            value = value.Replace("3GPP", string.Empty);
                            return StringToEnum<RFmxNRMXPdschDmrsReleaseVersion>(value);
                        }
                    };
                    [RfwsProperty("PTRS Ports", 4)]
                    public NrRfwsKey2 PtrsPorts = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschPtrsAntennaPorts,
                        RfmxType = RmfxPropertyTypes.String,
                    };
                    [RfwsProperty("PTRS Time Density", 4)]
                    public NrRfwsKey2 PtrsTimeDensity = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschPtrsTimeDensity,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("PTRS Frequency Density", 4)]
                    public NrRfwsKey2 PtrsFrequencyDensity = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschPtrsFrequencyDensity,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("DL PTRS RE Offset", 4)]
                    public NrRfwsKey2 PtrsReOffset = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschPtrsREOffset,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("PTRS Enabled", 4)]
                    public NrRfwsKey2 PtrsEnabled = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschPtrsEnabled,
                        RfmxType = RmfxPropertyTypes.Bool,
                    };
                    [RfwsProperty("PTRS Power Mode", 4)]
                    public NrRfwsKey2 PtrsPowerMode = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschPtrsPowerMode,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPdschPtrsPowerMode>(value)
                    };
                    [RfwsProperty("PTRS Scaling Factor", 4)]
                    public NrRfwsKey2 PtrsPower = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschPtrsPower,
                        RfmxType = RmfxPropertyTypes.Double,
                        CustomMap = (value) => ValueTodB(value)
                    };
                    [RfwsProperty("DMRS Ports", 4)]
                    public NrRfwsKey2 DmrsPorts = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsAntennaPorts,
                        RfmxType = RmfxPropertyTypes.String
                    };
                }

                // Section has number in title
                [RfwsSection(@"PUSCH Slot Settings \d+", version = "6", regExMatch = true)]
                public class PuschSlotSettings : RfwsSection<RFmxNRMX>
                {
                    public const string KeyPuschSlotIndex = "Array Index";

                    public PuschSlotSettings(XElement root, XElement section, RfwsParser<RFmxNRMX> parser, RFmxNRMX signal, string selectorString)
                        : base(root, section, parser, signal, selectorString)
                    {
                        int puschIndex = int.Parse(FetchValue(SectionRoot, KeyPuschSlotIndex));
                        Signal.ComponentCarrier.SetNumberOfPuschConfigurations(SelectorString, puschIndex + 1);
                        SelectorString = RFmxNRMX.BuildPuschString(SelectorString, puschIndex);
                    }
                    [RfwsProperty("Slot Allocation", 4)]
                    public NrRfwsKey2 RbAllocation = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschSlotAllocation,
                        RfmxType = RmfxPropertyTypes.String
                    };
                    [RfwsProperty("Symbol Allocation", 4)]
                    public NrRfwsKey2 SymbolAllocation = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschSymbolAllocation,
                        RfmxType = RmfxPropertyTypes.String
                    };
                    [RfwsProperty("Modulation Type", 4)]
                    public NrRfwsKey2 ModulationType = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschModulationType,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPuschModulationType>(value)
                    };
                    [RfwsProperty("PUSCH Mapping Type", 4)]
                    public NrRfwsKey2 MappingType = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschMappingType,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPuschMappingType>(value)
                    };
                    [RfwsProperty("DMRS Duration", 4)]
                    public NrRfwsKey2 DmrsDuration = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsDuration,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPuschDmrsDuration>(value)
                    };
                    [RfwsProperty("DMRS Configuration Type", 4)]
                    public NrRfwsKey2 DmrsConfiguration = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsConfigurationType,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPuschDmrsConfigurationType>(value)
                    };
                    [RfwsProperty("DMRS Power Mode", 4)]
                    public NrRfwsKey2 DmrsPowerMode = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsPowerMode,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPuschDmrsPowerMode>(value)
                    };
                    [RfwsProperty("DMRS Scaling Factor", 4)]
                    public NrRfwsKey2 DmrsPower = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsPower,
                        RfmxType = RmfxPropertyTypes.Double,
                        CustomMap = (value) => ValueTodB(value)
                    };
                    [RfwsProperty("DMRS Additional Positions", 4)]
                    public NrRfwsKey2 AdditionalPositions = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsAdditionalPositions,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("DMRS Type A Position", 4)]
                    public NrRfwsKey2 TypeAPosition = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsTypeAPosition,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("Transform Precoding Enabled", 4)]
                    public NrRfwsKey2 TransformPreCodingEnabled = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschTransformPrecodingEnabled,
                        RfmxType = RmfxPropertyTypes.Bool,
                    };
                    [RfwsProperty("PTRS Time Density", 4)]
                    public NrRfwsKey2 PtrsTimeDensity = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschPtrsTimeDensity,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("PTRS Frequency Density", 4)]
                    public NrRfwsKey2 PtrsFrequencyDensity = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschPtrsFrequencyDensity,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("UL PTRS RE Offset", 4)]
                    public NrRfwsKey2 PtrsReOffset = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschPtrsREOffset,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("PTRS Enabled", 4)]
                    public NrRfwsKey2 PtrsEnabled = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschPtrsEnabled,
                        RfmxType = RmfxPropertyTypes.Bool,
                    };

                    [RfwsProperty("DMRS Scrambling ID", 4)]
                    public NrRfwsKey2 ScramblingId = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsScramblingID,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("DMRS Scrambling ID Mode", 4)]
                    public NrRfwsKey2 ScramblingMode = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsScramblingIDMode,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPuschDmrsScramblingIDMode>(value)
                    };
                    [RfwsProperty("Dmrs Release Version", 6)]
                    public NrRfwsKey2 ReleaseVersion = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsReleaseVersion,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) =>
                        {
                            value = value.Replace("3GPP", string.Empty);
                            return StringToEnum<RFmxNRMXPuschDmrsReleaseVersion>(value);
                        }
                    };
                    [RfwsProperty("PTRS Power Mode", 4)]
                    public NrRfwsKey2 PtrsPowerMode = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschPtrsPowerMode,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPuschPtrsPowerMode>(value)
                    };
                    [RfwsProperty("PTRS Scaling Factor", 4)]
                    public NrRfwsKey2 PtrsPower = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschPtrsPower,
                        RfmxType = RmfxPropertyTypes.Double,
                        CustomMap = (value) => ValueTodB(value)
                    };
                    [RfwsProperty("PUSCH ID", 4)]
                    public NrRfwsKey2 DrmsPuschId = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsPuschID,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("PUSCH ID Mode", 4)]
                    public NrRfwsKey2 DrmsPuschIdMode = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsPuschIDMode,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPuschDmrsPuschIDMode>(value)
                    };
                    [RfwsProperty("Number of CDM Groups", 4)]
                    public NrRfwsKey2 CdmGroups = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsNumberOfCdmGroups,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("DMRS Ports", 4)]
                    public NrRfwsKey2 DmrsPorts = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsAntennaPorts,
                        RfmxType = RmfxPropertyTypes.String
                    };
                    [RfwsProperty("PTRS Ports", 4)]
                    public NrRfwsKey2 PtrsPorts = new NrRfwsKey
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschPtrsAntennaPorts,
                        RfmxType = RmfxPropertyTypes.String,
                    };
                }
            }
        }*/
    }

}
