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

                //Console.WriteLine($"Configuring subblock {subblockIndex}, component carrier {ccIndex}");
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
        // Current version (20.0) is 5, but properties below also work with 19.1 (version 3)
        [RfwsSection("Cell Settings", version = "5")]
        public class CellSettings : RfwsSection<RFmxNRMX>
        {
            public CellSettings(XElement childSection, RfwsSection<RFmxNRMX> parentSection)
                : base(childSection, parentSection) { }

            [RfwsProperty("Cell ID", 3)]
            public NrRfwsKey<int> CellId = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.CellID,
            };
            [RfwsProperty("Bandwidth (Hz)", 3)]
            public NrRfwsKey<double> Bandwidth = new NrRfwsKey<double>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.ComponentCarrierBandwidth,
            };
            [RfwsProperty("Frequency Range", 3)]
            public NrRfwsKey<int> FrequencyRange = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.FrequencyRange,
                SelectorStringType = RfmxNrSelectorStringType.Subblock,
                CustomMap = (value) => (int)StringToEnum<RFmxNRMXFrequencyRange>(value)
            };
            [RfwsProperty("Reference Grid Alignment Mode", 3)]
            public static NrRfwsKey<int> RefGridAlignmentMode = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.ReferenceGridAlignmentMode,
                SelectorStringType = RfmxNrSelectorStringType.None,
                CustomMap = (value) => (int)StringToEnum<RFmxNRMXReferenceGridAlignmentMode>(value)
            };
            [RfwsProperty("Reference Grid Subcarrier Spacing", 3)]
            public NrRfwsKey<double> RefGridSubcarrierSpacing = new NrRfwsKey<double>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.ReferenceGridSubcarrierSpacing,
            };
            [RfwsProperty("Reference Grid Start", 3)]
            public NrRfwsKey<int> ReferenceGridStart = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.ReferenceGridStart,
            };
            // This key is only located here in RFmx 19.1; it is moved to the CarrierSet section in 20.0
            [RfwsProperty("Auto Increment Cell ID Enabled", 3, RfswVersionMode.SpecificVersions)]
            public NrRfwsKey<bool> AutoIncrementCellId_19_1 = new NrRfwsKey<bool>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.AutoIncrementCellIDEnabled,
                SelectorStringType = RfmxNrSelectorStringType.None
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
        }

        [RfwsSection(@"Ssb Settings", version = "4")]
        public class SsbSettings : RfwsSection<RFmxNRMX>
        {
            public SsbSettings(XElement childSection, RfwsSection<RFmxNRMX> parentSection)
                : base(childSection, parentSection) { }
            
            [RfwsProperty("Configuration Set", 4)]
            public NrRfwsKey<int> SsbPattern = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.SsbPattern,
                CustomMap = (value) =>
                {
                    value = value.Replace("-", string.Empty);
                    value = value.Replace("3up", "3GHz");
                    return (int)StringToEnum<RFmxNRMXSsbPattern>(value);
                }
            };
            [RfwsProperty("SSS Scaling Factor", 4)]
            public NrRfwsKey<double> SssPower = new NrRfwsKey<double>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.SssPower,
                CustomMap = (value) => ValueTodB(value),
            };
            [RfwsProperty("PSS Scaling Factor", 4)]
            public NrRfwsKey<double> PssPower = new NrRfwsKey<double>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.PssPower,
                CustomMap = (value) => ValueTodB(value),
            };
            [RfwsProperty("PBCH Scaling Factor", 4)]
            public NrRfwsKey<double> PbchPower = new NrRfwsKey<double>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.PbchPower,
                CustomMap = (value) => ValueTodB(value),
            };
            [RfwsProperty("PBCH DMRS Scaling Factor", 4)]
            public NrRfwsKey<double> PbchDrmsPower = new NrRfwsKey<double>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.PbchDmrsPower,
                CustomMap = (value) => ValueTodB(value),
            };
            [RfwsProperty("Subcarrier Spacing Common", 4)]
            public NrRfwsKey<double> SubcarrierSpacingCommon = new NrRfwsKey<double>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.SubcarrierSpacingCommon,
            };
            [RfwsProperty("Subcarrier Offset", 4)]
            public NrRfwsKey<int> SubcarrierOffset = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.SsbSubcarrierOffset,
            };
            [RfwsProperty("Periodicity", 4)]
            public NrRfwsKey<double> SsbPeriodicity = new NrRfwsKey<double>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.SsbPeriodicity,
            };
            [RfwsProperty("SSB Active Blocks", 4)]
            public NrRfwsKey<string> SsbActiveBlocks = new NrRfwsKey<string>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.SsbActiveBlocks,
            };
        }

        // Bandwidth Part Section has number in title
        [RfwsSection(@"Bandwidth Part Settings \d+", version = "3", regExMatch = true)]
        public class BandwidthPartSettings : RfwsSection<RFmxNRMX>
        {
            public const string KeyBandwidthPartIndex = "Bandwidth Part Index";

            public BandwidthPartSettings(XElement childSection, RfwsSection<RFmxNRMX> parentSection)
                : base(childSection, parentSection)
            {
                int bandwidthPartIndex = int.Parse(FetchValue(SectionRoot, KeyBandwidthPartIndex));
                Signal.ComponentCarrier.SetNumberOfBandwidthParts(SelectorString, bandwidthPartIndex + 1);
                SelectorString = RFmxNRMX.BuildBandwidthPartString(SelectorString, bandwidthPartIndex);
            }


            [RfwsProperty("Subcarrier Spacing (Hz)", 3)]
            public NrRfwsKey<double> SubcarrierSpacing = new NrRfwsKey<double>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.BandwidthPartSubcarrierSpacing,
            };
            [RfwsProperty("Cyclic Prefix Mode", 3)]
            public NrRfwsKey<int> CyclicPrefixMode = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.BandwidthPartCyclicPrefixMode,
                CustomMap = (value) => (int)StringToEnum<RFmxNRMXBandwidthPartCyclicPrefixMode>(value)
            };
            [RfwsProperty("Grid Start", 3)]
            public NrRfwsKey<int> GridStart = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.GridStart,
            };
            [RfwsProperty("RB Offset", 3)]
            public NrRfwsKey<int> RbOffset = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.BandwidthPartResourceBlockOffset,
            };
            [RfwsProperty("Number of RBs", 3)]
            public NrRfwsKey<int> NumberOfRbs = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.BandwidthPartNumberOfResourceBlocks,
            };
            [RfwsProperty("UE Count", 3)]
            public NrRfwsKey<int> NumberOfUe = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.NumberOfUsers,
            };
            /*[RfwsProperty("Coreset Count", 3)]
            public NrRfwsKey<int> NumberOfCoreset = new NrRfwsKey<int>
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.NumberOfCoresets,
            };*/// Set in Coreset section

            // Section has number in title
            [RfwsSection(@"UE Settings \d+", version = "1", regExMatch = true)]
            public class UeSettings : RfwsSection<RFmxNRMX>
            {
                public const string KeyUeIndex = "UE Index";

                public UeSettings(XElement childSection, RfwsSection<RFmxNRMX> parentSection)
                : base(childSection, parentSection)
                {
                    int ueIndex = int.Parse(FetchValue(SectionRoot, KeyUeIndex));
                    SelectorString = RFmxNRMX.BuildUserString(SelectorString, ueIndex);
                }

                [RfwsProperty("nRNTI", 1)]
                public NrRfwsKey<int> Rnti = new NrRfwsKey<int>
                {
                    RfmxPropertyId = (int)RFmxNRMXPropertyId.Rnti,
                };

                // Section has number in title
                [RfwsSection(@"PDSCH Slot Settings \d+", version = "4", regExMatch = true)]
                public class PdschSlotSettings : RfwsSection<RFmxNRMX>
                {
                    public const string KeyPdschSlotIndex = "Array Index";

                    public PdschSlotSettings(XElement childSection, RfwsSection<RFmxNRMX> parentSection)
                        : base(childSection, parentSection)
                    {
                        int pdschIndex = int.Parse(FetchValue(SectionRoot, KeyPdschSlotIndex));
                        Signal.ComponentCarrier.SetNumberOfPdschConfigurations(SelectorString, pdschIndex + 1);
                        SelectorString = RFmxNRMX.BuildPdschString(SelectorString, pdschIndex);
                    }

                    [RfwsProperty("PDSCH Present in SSB RB", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<bool> PdschPressentInSsbRb = new NrRfwsKey<bool>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschPresentInSsbResourceBlock,
                    };
                    [RfwsProperty("Slot Allocation", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<string> RbAllocation = new NrRfwsKey<string>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschSlotAllocation,
                    };
                    [RfwsProperty("Symbol Allocation", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<string> SymbolAllocation = new NrRfwsKey<string>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschSymbolAllocation,
                    };
                    [RfwsProperty("Modulation Type", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<int> ModulationType = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschModulationType,
                        CustomMap = (value) => (int)StringToEnum<RFmxNRMXPdschModulationType>(value)
                    };
                    [RfwsProperty("PDSCH Mapping Type", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<int> MappingType = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschMappingType,
                        CustomMap = (value) => (int)StringToEnum<RFmxNRMXPdschMappingType>(value)
                    };
                    [RfwsProperty("DMRS Duration", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<int> DmrsDuration = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsDuration,
                        CustomMap = (value) => (int)StringToEnum<RFmxNRMXPdschDmrsDuration>(value)
                    };
                    [RfwsProperty("DMRS Configuration", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<int> DmrsConfiguration = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsConfigurationType,
                        CustomMap = (value) => (int)StringToEnum<RFmxNRMXPdschDmrsConfigurationType>(value)
                    };
                    [RfwsProperty("DMRS Power Mode", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<int> DmrsPowerMode = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsPowerMode,
                        CustomMap = (value) => (int)StringToEnum<RFmxNRMXPdschDmrsPowerMode>(value)
                    };
                    [RfwsProperty("DMRS Scaling Factor", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<double> DmrsPower = new NrRfwsKey<double>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsPower,
                        CustomMap = (value) => ValueTodB(value)
                    };
                    [RfwsProperty("DMRS Additional Positions", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<int> AdditionalPositions = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsAdditionalPositions,
                    };
                    [RfwsProperty("DMRS Type A Position", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<int> TypeAPosition = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsTypeAPosition,
                    };
                    [RfwsProperty("DMRS Scrambling ID", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<int> ScramblingId = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsScramblingID,
                    };
                    [RfwsProperty("DMRS Scrambling ID Mode", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<int> ScramblingMode = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsScramblingIDMode,
                        CustomMap = (value) => (int)StringToEnum<RFmxNRMXPdschDmrsScramblingIDMode>(value)
                    };
                    [RfwsProperty("Number of CDM Groups", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<int> CdmGroups = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsNumberOfCdmGroups,
                    };
                    [RfwsProperty("nSCID", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<int> Nscid = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsnScid,
                    };
                    [RfwsProperty("Dmrs Release Version", 4)]
                    public NrRfwsKey<int> ReleaseVersion = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsReleaseVersion,
                        CustomMap = (value) =>
                        {
                            value = value.Replace("3GPP", string.Empty);
                            return (int)StringToEnum<RFmxNRMXPdschDmrsReleaseVersion>(value);
                        }
                    };
                    [RfwsProperty("PTRS Ports", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<string> PtrsPorts = new NrRfwsKey<string>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschPtrsAntennaPorts,
                    };
                    [RfwsProperty("PTRS Time Density", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<int> PtrsTimeDensity = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschPtrsTimeDensity,
                    };
                    [RfwsProperty("PTRS Frequency Density", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<int> PtrsFrequencyDensity = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschPtrsFrequencyDensity,
                    };
                    [RfwsProperty("DL PTRS RE Offset", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<int> PtrsReOffset = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschPtrsREOffset,
                    };
                    [RfwsProperty("PTRS Enabled", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<bool> PtrsEnabled = new NrRfwsKey<bool>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschPtrsEnabled,
                    };
                    [RfwsProperty("PTRS Power Mode", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<int> PtrsPowerMode = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschPtrsPowerMode,
                        CustomMap = (value) => (int)StringToEnum<RFmxNRMXPdschPtrsPowerMode>(value)
                    };
                    [RfwsProperty("PTRS Scaling Factor", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<double> PtrsPower = new NrRfwsKey<double>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschPtrsPower,
                        CustomMap = (value) => ValueTodB(value)
                    };
                    [RfwsProperty("DMRS Ports", RfswVersionMode.SupportedVersionsAndLater, 3, 4)]
                    public NrRfwsKey<string> DmrsPorts = new NrRfwsKey<string>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsAntennaPorts,
                    };
                }

                // Section has number in title
                [RfwsSection(@"PUSCH Slot Settings \d+", version = "6", regExMatch = true)]
                public class PuschSlotSettings : RfwsSection<RFmxNRMX>
                {
                    public const string KeyPuschSlotIndex = "Array Index";

                    public PuschSlotSettings(XElement childSection, RfwsSection<RFmxNRMX> parentSection)
                        : base(childSection, parentSection)
                    {
                        int puschIndex = int.Parse(FetchValue(SectionRoot, KeyPuschSlotIndex));
                        Signal.ComponentCarrier.SetNumberOfPuschConfigurations(SelectorString, puschIndex + 1);
                        SelectorString = RFmxNRMX.BuildPuschString(SelectorString, puschIndex);
                    }
                    [RfwsProperty("Slot Allocation", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<string> RbAllocation = new NrRfwsKey<string>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschSlotAllocation,
                    };
                    [RfwsProperty("Symbol Allocation", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<string> SymbolAllocation = new NrRfwsKey<string>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschSymbolAllocation,
                    };
                    [RfwsProperty("Modulation Type", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<int> ModulationType = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschModulationType,
                        CustomMap = (value) => (int)StringToEnum<RFmxNRMXPuschModulationType>(value)
                    };
                    [RfwsProperty("PUSCH Mapping Type", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<int> MappingType = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschMappingType,
                        CustomMap = (value) => (int)StringToEnum<RFmxNRMXPuschMappingType>(value)
                    };
                    [RfwsProperty("DMRS Duration", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<int> DmrsDuration = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsDuration,
                        CustomMap = (value) => (int)StringToEnum<RFmxNRMXPuschDmrsDuration>(value)
                    };
                    [RfwsProperty("DMRS Configuration Type", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<int> DmrsConfiguration = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsConfigurationType,
                        CustomMap = (value) => (int)StringToEnum<RFmxNRMXPuschDmrsConfigurationType>(value)
                    };
                    [RfwsProperty("DMRS Power Mode", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<int> DmrsPowerMode = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsPowerMode,
                        CustomMap = (value) => (int)StringToEnum<RFmxNRMXPuschDmrsPowerMode>(value)
                    };
                    [RfwsProperty("DMRS Scaling Factor", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<double> DmrsPower = new NrRfwsKey<double>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsPower,
                        CustomMap = (value) => ValueTodB(value)
                    };
                    [RfwsProperty("DMRS Additional Positions", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<int> AdditionalPositions = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsAdditionalPositions,
                    };
                    [RfwsProperty("DMRS Type A Position", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<int> TypeAPosition = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsTypeAPosition,
                    };
                    [RfwsProperty("Transform Precoding Enabled", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<bool> TransformPreCodingEnabled = new NrRfwsKey<bool>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschTransformPrecodingEnabled,
                    };
                    [RfwsProperty("PTRS Time Density", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<int> PtrsTimeDensity = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschPtrsTimeDensity,
                    };
                    [RfwsProperty("PTRS Frequency Density", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<int> PtrsFrequencyDensity = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschPtrsFrequencyDensity,
                    };
                    [RfwsProperty("UL PTRS RE Offset", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<int> PtrsReOffset = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschPtrsREOffset,
                    };
                    [RfwsProperty("PTRS Enabled", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<bool> PtrsEnabled = new NrRfwsKey<bool>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschPtrsEnabled,
                    };

                    [RfwsProperty("DMRS Scrambling ID", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<int> ScramblingId = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsScramblingID,
                    };
                    [RfwsProperty("DMRS Scrambling ID Mode", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<int> ScramblingMode = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsScramblingIDMode,
                        CustomMap = (value) => (int)StringToEnum<RFmxNRMXPuschDmrsScramblingIDMode>(value)
                    };
                    [RfwsProperty("Dmrs Release Version", 6)]
                    public NrRfwsKey<int> ReleaseVersion = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsReleaseVersion,
                        CustomMap = (value) =>
                        {
                            value = value.Replace("3GPP", string.Empty);
                            return (int)StringToEnum<RFmxNRMXPuschDmrsReleaseVersion>(value);
                        }
                    };
                    [RfwsProperty("PTRS Power Mode", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<int> PtrsPowerMode = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschPtrsPowerMode,
                        CustomMap = (value) => (int)StringToEnum<RFmxNRMXPuschPtrsPowerMode>(value)
                    };
                    [RfwsProperty("PTRS Scaling Factor", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<double> PtrsPower = new NrRfwsKey<double>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschPtrsPower,
                        CustomMap = (value) => ValueTodB(value)
                    };
                    [RfwsProperty("PUSCH ID", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<int> DrmsPuschId = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsPuschID,
                    };
                    [RfwsProperty("PUSCH ID Mode", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<int> DrmsPuschIdMode = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsPuschIDMode,
                        CustomMap = (value) => (int)StringToEnum<RFmxNRMXPuschDmrsPuschIDMode>(value)
                    };
                    [RfwsProperty("Number of CDM Groups", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<int> CdmGroups = new NrRfwsKey<int>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsNumberOfCdmGroups,
                    };
                    [RfwsProperty("DMRS Ports", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<string> DmrsPorts = new NrRfwsKey<string>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschDmrsAntennaPorts,
                    };
                    [RfwsProperty("PTRS Ports", RfswVersionMode.SupportedVersionsAndLater, 3, 6)]
                    public NrRfwsKey<string> PtrsPorts = new NrRfwsKey<string>
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PuschPtrsAntennaPorts,
                    };
                }
            }

            /* TO BE COMPLETED AT A LATER DATE
            [RfwsSection(@"CORESET Settings \d+", version = "1", regExMatch = true)]
            public class CoresetSettings : RfwsSection<RFmxNRMX>
            {
                public const string KeyCoresetIndex = "Coreset Index";

                public CoresetSettings(XElement childSection, RfwsSection<RFmxNRMX> parentSection)
                    : base(childSection, parentSection)
                {
                    int coreSetIndex = int.Parse(FetchValue(SectionRoot, KeyCoresetIndex));
                    Signal.ComponentCarrier.SetNumberOfCoresets(SelectorString, coreSetIndex + 1);
                    SelectorString = RFmxNRMX.BuildCoresetString(SelectorString, coreSetIndex);
                }
                [RfwsProperty("Coreset Num Symbols", 1)]
                public NrRfwsKey<int> NumberOfCoreset = new NrRfwsKey<int>
                {
                    RfmxPropertyId = (int)RFmxNRMXPropertyId.CoresetNumberOfSymbols,
                };
                [RfwsProperty("Coreset Num Symbols", 1)]
                public NrRfwsKey<int> NumberOfCoreset = new NrRfwsKey<int>
                {
                    RfmxPropertyId = (int)RFmxNRMXPropertyId.Coreset,
                };
            }*/
        }
    }

}
