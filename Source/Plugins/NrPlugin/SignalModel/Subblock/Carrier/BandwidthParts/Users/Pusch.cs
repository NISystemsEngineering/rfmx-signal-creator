using System;
using System.Xml.Linq;
using NationalInstruments.RFmx.NRMX;
using System.Collections.Generic;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using RfwsParser.Converters;
    using SignalCreator.RfwsParser;

    public class PuschSettings
    {
        [RfwsParseableKey("Count", 1), RFmxNrMappableProperty(RFmxNRMXPropertyId.NumberOfPuschConfigurations)]
        public int? NumPusch;

        [RfwsSection(@"PUSCH Slot Settings \d+", version = "6", regExMatch = true)]
        [RFmxMappableSection(SelectorStrings.Pusch)]
        public List<Pusch> PuschConfigurations;
    }

    public class Pusch 
    {

        [RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschNumberOfResourceBlockClusters)]
        public int? NumberOfRbClusters => PuschRbClusters?.Count;

        #region Section Version 3 (Pre 20.0 release and above)
        [RfwsParseableKey("RB Allocation", 3, ConverterType = typeof(RbClusterConverter<PuschRbCluster>))]
        [RFmxMappableSection(SelectorStrings.PuschCluster)]
        public List<PuschRbCluster> PuschRbClusters;
        [RfwsParseableKey("Slot Allocation", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschSlotAllocation)]
        public string RbAllocation;
        [RfwsParseableKey("Symbol Allocation", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschSymbolAllocation)]
        public string SymbolAllocation;
        [RfwsParseableKey("Modulation Type", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschModulationType)]
        public RFmxNRMXPuschModulationType? ModulationType;
        [RfwsParseableKey("PUSCH Mapping Type", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschMappingType)]
        public RFmxNRMXPuschMappingType? MappingType;
        [RfwsParseableKey("DMRS Duration", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschDmrsDuration)]
        public RFmxNRMXPuschDmrsDuration? DmrsDuration;
        [RfwsParseableKey("DMRS Configuration Type", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschDmrsConfigurationType)]
        public RFmxNRMXPuschDmrsConfigurationType? DmrsConfiguration;
        [RfwsParseableKey("DMRS Power Mode", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschDmrsPowerMode)]
        public RFmxNRMXPuschDmrsPowerMode? DmrsPowerMode;
        [RfwsParseableKey("DMRS Scaling Factor", 3, ConverterType = typeof(LinearTodBConverter))]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschDmrsPower)]
        public double? DmrsPower;
        [RfwsParseableKey("DMRS Additional Positions", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschDmrsAdditionalPositions)]
        public int? AdditionalPositions;
        [RfwsParseableKey("DMRS Type A Position", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschDmrsTypeAPosition)]
        public int? TypeAPosition;
        [RfwsParseableKey("Transform Precoding Enabled", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschTransformPrecodingEnabled)]
        public bool? TransformPreCodingEnabled;
        [RfwsParseableKey("PTRS Time Density", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschPtrsTimeDensity)]
        public int? PtrsTimeDensity;
        [RfwsParseableKey("PTRS Frequency Density", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschPtrsFrequencyDensity)]
        public int? PtrsFrequencyDensity;
        [RfwsParseableKey("UL PTRS RE Offset", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschPtrsREOffset)]
        public int? PtrsReOffset;
        [RfwsParseableKey("PTRS Enabled", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschPtrsEnabled)]
        public bool? PtrsEnabled;
        [RfwsParseableKey("DMRS Scrambling ID", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschDmrsScramblingID)]
        public int? ScramblingId;
        [RfwsParseableKey("DMRS Scrambling ID Mode", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschDmrsScramblingIDMode)]
        public RFmxNRMXPuschDmrsScramblingIDMode? ScramblingMode;
        // PUSCH DMRS Release Version
        [RfwsParseableKey("PTRS Power Mode", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschPtrsPowerMode)]
        public RFmxNRMXPuschPtrsPowerMode? PtrsPowerMode;
        [RfwsParseableKey("PTRS Scaling Factor", 3, ConverterType = typeof(LinearTodBConverter))]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschPtrsPower)]
        public double? PtrsPower;
        [RfwsParseableKey("PUSCH ID", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschDmrsPuschID)]
        public int? DrmsPuschId;
        [RfwsParseableKey("PUSCH ID Mode", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschDmrsPuschIDMode)]
        public RFmxNRMXPuschDmrsPuschIDMode? DrmsPuschIdMode;
        [RfwsParseableKey("Number of CDM Groups", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschDmrsNumberOfCdmGroups)]
        public int? CdmGroups;
        [RfwsParseableKey("DMRS Ports", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschDmrsAntennaPorts)]
        public string DmrsPorts;
        [RfwsParseableKey("PTRS Ports", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschPtrsAntennaPorts)]
        public string PtrsPorts;
        #endregion

        #region Section Version 6 (20.0 release)
        public class PuschDrmsReleaseVersionConverter : EnumConverter<RFmxNRMXPuschDmrsReleaseVersion>
        {
            protected override RFmxNRMXPuschDmrsReleaseVersion Convert(object value)
            {
                string textValue = (string)value;
                textValue = textValue.Replace("3GPP", string.Empty);
                return base.Convert(textValue);
            }
        }

        [RfwsParseableKey("Dmrs Release Version", 6, ConverterType = typeof(PuschDrmsReleaseVersionConverter))]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.PuschDmrsReleaseVersion)]
        public RFmxNRMXPuschDmrsReleaseVersion? ReleaseVersion;
        #endregion
    }



}
