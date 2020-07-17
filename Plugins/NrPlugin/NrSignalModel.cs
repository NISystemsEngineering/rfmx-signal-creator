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
    public interface IParseable
    {
        void Parse();
    }

    public enum RfmxSelectorStringType
    {
        Default,
        Subblock,
        Carrier,
        None
    }
    public class NrRfwsRfmxPropertyMap : RfwsRfmxPropertyMap
    {
        public RfmxSelectorStringType SelectorStringType;
    }

    public abstract class NrSignalModel : IParseable
    {
        protected RFmxNRMX _nrSignal;
        protected string _selectorString;
        protected XElement _root;

        protected NrSignalModel(RFmxNRMX nrSignal, string selectorString, XElement root)
        {
            _nrSignal = nrSignal;
            _selectorString = selectorString;
            _root = root;
        }

        public abstract void Parse();
    }

    [RfwsSection("Carrier", version = "3")]
    public class Subblock : RfwsSection<RFmxNRMX>// : NrSignalModel
    {
        public const string KeySubblockNumber = "CarrierSubblockNumber";
        public const string KeyCarrierDefinition = "CarrierDefinition";
        public const string KeyCarrierCCIndex = "CarrierCCIndex";

        public Subblock(XElement root, XElement section, RFmxNRMX signal, string selectorString)
            : base(root, section, signal, selectorString)
        {
            // Build selector string for the current subblock and CC
            int subblockIndex = int.Parse(FetchValue(section, KeySubblockNumber));

            signal.SetNumberOfSubblocks("", subblockIndex + 1);

            SelectorString = RFmxNRMX.BuildSubblockString(SelectorString, subblockIndex);

            // RFmx WC defines the subblocks only in relative frequency offsets
            signal.SetSubblockFrequencyDefinition(SelectorString, RFmxNRMXSubblockFrequencyDefinition.Relative);

            int ccIndex = int.Parse(FetchValue(section, KeyCarrierCCIndex));
            signal.ComponentCarrier.SetNumberOfComponentCarriers(SelectorString, ccIndex + 1);

            SelectorString = RFmxNRMX.BuildCarrierString(SelectorString, ccIndex);


            Console.WriteLine($"Configuring subblock {subblockIndex}, component carrier {ccIndex}");
        }

        [RfwsProperty("CarrierSubblockOffset", 3)]
        public static NrRfwsRfmxPropertyMap CarrierSubblockOffset = new NrRfwsRfmxPropertyMap
        {
            RfmxPropertyId = (int)RFmxNRMXPropertyId.CenterFrequency,
            RfmxType = RmfxPropertyTypes.Double,
            SelectorStringType = RfmxSelectorStringType.Subblock,
        };
        [RfwsProperty("CarrierFrequencyOffset", 3)]
        public static NrRfwsRfmxPropertyMap CarrierFrequencyOffset = new NrRfwsRfmxPropertyMap
        {
            RfmxPropertyId = (int)RFmxNRMXPropertyId.ComponentCarrierFrequency,
            RfmxType = RmfxPropertyTypes.Double,

        };
        /*
        public Subblock(RFmxNRMX nrSignal, string selectorString, XElement root)
            : base(nrSignal, selectorString, root) { }

        public override void Parse()
        {
            ParseAndMapProperties()
            try
            {
                // Fetch the carrier definition number in order to reference the appropriate carrier in a moment
                string carrierDefinition = RfwsParserUtilities.FetchValue(element, KeyCarrierDefinition);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error parsing subblock.", ex);
            }
        }*/
    }

    [RfwsSection("CarrierDefinition", version = "1")]
    public class Carrier : RfwsSection<RFmxNRMX>
    {

        public Carrier(XElement root, XElement section, RFmxNRMX signal, string selectorString)
            : base(root, section, signal, selectorString)
        {
            // Fetch the carrier definition number in order to reference the appropriate carrier in a moment
            string carrierDefinitionIndex = FetchValue(SectionRoot, Subblock.KeyCarrierDefinition);

            // Fetch the carrier definition section
            XElement carrierDefinition = FindSections(root, "CarrierDefinitionManager").First();
            // Fetch the specific carrier
            XElement specificCarrier = FindSections(carrierDefinition, carrierDefinitionIndex).First();

            SectionRoot = specificCarrier;
        }

        [RfwsSection("Cell Settings", version = "5")]
        public class CellSettings : RfwsSection<RFmxNRMX>
        {
            public CellSettings(XElement root, XElement section, RFmxNRMX signal, string selectorString)
                : base(root, section, signal, selectorString) { }

            [RfwsProperty("Cell ID", 5)]
            public static NrRfwsRfmxPropertyMap CellId = new NrRfwsRfmxPropertyMap
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.CellID,
                RfmxType = RmfxPropertyTypes.Int,
            };
            [RfwsProperty("Bandwidth (Hz)", 5)]
            public static NrRfwsRfmxPropertyMap Bandwidth = new NrRfwsRfmxPropertyMap
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.ComponentCarrierBandwidth,
                RfmxType = RmfxPropertyTypes.Double,
            };
            [RfwsProperty("Frequency Range", 5)]
            public static NrRfwsRfmxPropertyMap FrequencyRange = new NrRfwsRfmxPropertyMap
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.FrequencyRange,
                RfmxType = RmfxPropertyTypes.Int,
                SelectorStringType = RfmxSelectorStringType.Subblock,
                CustomMap = (value) => StringToEnum<RFmxNRMXFrequencyRange>(value)
            };
            /*[RfwsPropertyAttribute]
            public static NrRfwsRfmxPropertyMap RefGridAlignmentMode = new NrRfwsRfmxPropertyMap
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.ReferenceGridAlignmentMode,
                RfmxType = RmfxPropertyTypes.Int,
                RfwsTag = "Reference Grid Alignment Mode",
                SelectorStringType = RfmxSelectorStringType.Subblock,
                CustomMap = (value) => Enum.Parse(typeof(RFmxNRMXReferenceGridAlignmentMode), value)
            };*/
            [RfwsProperty("Reference Grid Subcarrier Spacing", 5)]
            public static NrRfwsRfmxPropertyMap RefGridSubcarrierSpacing = new NrRfwsRfmxPropertyMap
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.ReferenceGridSubcarrierSpacing,
                RfmxType = RmfxPropertyTypes.Double,
            };
            [RfwsProperty("Reference Grid Start", 5)]
            public static NrRfwsRfmxPropertyMap ReferenceGridStart = new NrRfwsRfmxPropertyMap
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.ReferenceGridStart,
                RfmxType = RmfxPropertyTypes.Int,
            };
        }
        [RfwsSection("Output Settings", version = "3")]
        public class OutputSettings : RfwsSection<RFmxNRMX>
        {
            public OutputSettings(XElement root, XElement section, RFmxNRMX signal, string selectorString)
                : base(root, section, signal, selectorString) { }

            [RfwsProperty("Link Direction", 3)]
            public static NrRfwsRfmxPropertyMap LinkDirection = new NrRfwsRfmxPropertyMap
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.LinkDirection,
                RfmxType = RmfxPropertyTypes.Int,
                CustomMap = (value) => StringToEnum<RFmxNRMXLinkDirection>(value),
                SelectorStringType = RfmxSelectorStringType.None
            };
            [RfwsProperty("DL Ch Configuration Mode", 3)]
            public static NrRfwsRfmxPropertyMap DlChannelConfigMode = new NrRfwsRfmxPropertyMap
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.DownlinkChannelConfigurationMode,
                RfmxType = RmfxPropertyTypes.Int,
                CustomMap = (value) =>
                    StringToEnum<RFmxNRMXDownlinkChannelConfigurationMode>(value),
                SelectorStringType = RfmxSelectorStringType.None
            };
            [RfwsProperty("DL Test Model", 3)]
            public static NrRfwsRfmxPropertyMap DlTestModel = new NrRfwsRfmxPropertyMap
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.DownlinkTestModel,
                RfmxType = RmfxPropertyTypes.Int,
                CustomMap = (value) =>
                {
                    value = value.Replace(".", "_");
                    return StringToEnum<RFmxNRMXDownlinkTestModel>(value);
                },
                SelectorStringType = RfmxSelectorStringType.None
            };
            [RfwsProperty("DL Test Model Duplex Scheme", 3)]
            public static NrRfwsRfmxPropertyMap DlTestModelDuplexScheme = new NrRfwsRfmxPropertyMap
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.DownlinkTestModelDuplexScheme,
                RfmxType = RmfxPropertyTypes.Int,
                CustomMap = (value) => StringToEnum<RFmxNRMXDownlinkTestModelDuplexScheme>(value),
                SelectorStringType = RfmxSelectorStringType.None
            };
            // Not supporting for right now
            #region SSB Settings

            #endregion
        }
        // Bandwidth Part Section has number in title
        [RfwsSection(@"Bandwidth Part Settings \d+", version = "3", regExMatch = true)]
        public class BandwidthPartSettings : RfwsSection<RFmxNRMX>
        {
            public const string KeyBandwidthPartIndex = "Bandwidth Part Index";

            public BandwidthPartSettings(XElement root, XElement section, RFmxNRMX signal, string selectorString)
                : base(root, section, signal, selectorString)
            {
                int bandwidthPartIndex = int.Parse(FetchValue(SectionRoot, KeyBandwidthPartIndex));
                Signal.ComponentCarrier.SetNumberOfBandwidthParts(SelectorString, bandwidthPartIndex + 1);
                SelectorString = RFmxNRMX.BuildBandwidthPartString(SelectorString, bandwidthPartIndex);
            }


            [RfwsProperty("Subcarrier Spacing (Hz)", 3)]
            public static NrRfwsRfmxPropertyMap SubcarrierSpacing = new NrRfwsRfmxPropertyMap
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.BandwidthPartSubcarrierSpacing,
                RfmxType = RmfxPropertyTypes.Double,
            };
            [RfwsProperty("Cyclic Prefix Mode", 3)]
            public static NrRfwsRfmxPropertyMap CyclicPrefixMode = new NrRfwsRfmxPropertyMap
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.BandwidthPartCyclicPrefixMode,
                RfmxType = RmfxPropertyTypes.Int,
                CustomMap = (value) => StringToEnum<RFmxNRMXBandwidthPartCyclicPrefixMode>(value)
            };
            [RfwsProperty("Grid Start", 3)]
            public static NrRfwsRfmxPropertyMap GridStart = new NrRfwsRfmxPropertyMap
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.GridStart,
                RfmxType = RmfxPropertyTypes.Int
            };
            [RfwsProperty("RB Offset", 3)]
            public static NrRfwsRfmxPropertyMap RbOffset = new NrRfwsRfmxPropertyMap
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.BandwidthPartResourceBlockOffset,
                RfmxType = RmfxPropertyTypes.Int
            };
            [RfwsProperty("Number of RBs", 3)]
            public static NrRfwsRfmxPropertyMap NumberOfRbs = new NrRfwsRfmxPropertyMap
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.BandwidthPartNumberOfResourceBlocks,
                RfmxType = RmfxPropertyTypes.Int
            };
            [RfwsProperty("UE Count", 3)]
            public static NrRfwsRfmxPropertyMap NumberOfUe = new NrRfwsRfmxPropertyMap
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.NumberOfUsers,
                RfmxType = RmfxPropertyTypes.Int
            };
            [RfwsProperty("Coreset Count", 3)]
            public static NrRfwsRfmxPropertyMap NumberOfCoreset = new NrRfwsRfmxPropertyMap
            {
                RfmxPropertyId = (int)RFmxNRMXPropertyId.NumberOfCoresets,
                RfmxType = RmfxPropertyTypes.Int
            };

            // Section has number in title
            [RfwsSection(@"UE Settings \d+", version = "1", regExMatch = true)]
            public class UeSettings : RfwsSection<RFmxNRMX>
            {
                public const string KeyUeIndex = "UE Index";

                public UeSettings(XElement root, XElement section, RFmxNRMX signal, string selectorString)
                    : base(root, section, signal, selectorString)
                {
                    int ueIndex = int.Parse(FetchValue(SectionRoot, KeyUeIndex));
                    SelectorString = RFmxNRMX.BuildUserString(SelectorString, ueIndex);
                }

                [RfwsProperty("nRNTI", 1)]
                public static NrRfwsRfmxPropertyMap Rnti = new NrRfwsRfmxPropertyMap
                {
                    RfmxPropertyId = (int)RFmxNRMXPropertyId.Rnti,
                    RfmxType = RmfxPropertyTypes.Int
                };

                // Section has number in title
                [RfwsSection(@"PDSCH Slot Settings \d+", version = "4", regExMatch = true)]
                public class PdschSlotSettings : RfwsSection<RFmxNRMX>
                {
                    public const string KeyPdschSlotIndex = "Array Index";

                    public PdschSlotSettings(XElement root, XElement section, RFmxNRMX signal, string selectorString)
                        : base(root, section, signal, selectorString)
                    {
                        int pdschIndex = int.Parse(FetchValue(SectionRoot, KeyPdschSlotIndex));
                        Signal.ComponentCarrier.SetNumberOfPdschConfigurations(SelectorString, pdschIndex + 1);
                        SelectorString = RFmxNRMX.BuildPdschString(SelectorString, pdschIndex);
                    }

                    [RfwsProperty("PDSCH Present in SSB RB", 4)]
                    public static NrRfwsRfmxPropertyMap PdschPressentInSsbRb = new NrRfwsRfmxPropertyMap
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschPresentInSsbResourceBlock,
                        RfmxType = RmfxPropertyTypes.Bool
                    };
                    [RfwsProperty("Slot Allocation", 4)]
                    public static NrRfwsRfmxPropertyMap RbAllocation = new NrRfwsRfmxPropertyMap
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschSlotAllocation,
                        RfmxType = RmfxPropertyTypes.String
                    };
                    [RfwsProperty("Symbol Allocation", 4)]
                    public static NrRfwsRfmxPropertyMap SymbolAllocation = new NrRfwsRfmxPropertyMap
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschSymbolAllocation,
                        RfmxType = RmfxPropertyTypes.String
                    };
                    [RfwsProperty("Modulation Type", 4)]
                    public static NrRfwsRfmxPropertyMap ModulationType = new NrRfwsRfmxPropertyMap
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschModulationType,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPdschModulationType>(value)
                    };
                    [RfwsProperty("PDSCH Mapping Type", 4)]
                    public static NrRfwsRfmxPropertyMap MappingType = new NrRfwsRfmxPropertyMap
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschMappingType,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPdschMappingType>(value)
                    };
                    [RfwsProperty("DMRS Duration", 4)]
                    public static NrRfwsRfmxPropertyMap DrmsDuration = new NrRfwsRfmxPropertyMap
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsDuration,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPdschDmrsDuration>(value)
                    };
                    [RfwsProperty("DMRS Configuration", 4)]
                    public static NrRfwsRfmxPropertyMap DrmsConfiguration = new NrRfwsRfmxPropertyMap
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsConfigurationType,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPdschDmrsConfigurationType>(value)
                    };
                    [RfwsProperty("DMRS Power Mode", 4)]
                    public static NrRfwsRfmxPropertyMap PowerMode = new NrRfwsRfmxPropertyMap
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsPowerMode,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPdschDmrsPowerMode>(value)
                    };
                    [RfwsProperty("DMRS Additional Positions", 4)]
                    public static NrRfwsRfmxPropertyMap AdditionalPositions = new NrRfwsRfmxPropertyMap
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsAdditionalPositions,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("DMRS Type A Position", 4)]
                    public static NrRfwsRfmxPropertyMap TypeAPosition = new NrRfwsRfmxPropertyMap
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsTypeAPosition,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("DMRS Scrambling ID", 4)]
                    public static NrRfwsRfmxPropertyMap ScramblingId = new NrRfwsRfmxPropertyMap
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsScramblingID,
                        RfmxType = RmfxPropertyTypes.Int,
                    };
                    [RfwsProperty("DMRS Scrambling ID Mode", 4)]
                    public static NrRfwsRfmxPropertyMap ScramblingMode = new NrRfwsRfmxPropertyMap
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.PdschDmrsScramblingIDMode,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPdschDmrsScramblingIDMode>(value)
                    };
                    [RfwsProperty("DMRS Scrambling ID Mode", 4)]
                    public static NrRfwsRfmxPropertyMap ScramblingMode = new NrRfwsRfmxPropertyMap
                    {
                        RfmxPropertyId = (int)RFmxNRMXPropertyId.ptrs,
                        RfmxType = RmfxPropertyTypes.Int,
                        CustomMap = (value) => StringToEnum<RFmxNRMXPdschDmrsScramblingIDMode>(value)
                    };
                }

            }
        }
    }

}
