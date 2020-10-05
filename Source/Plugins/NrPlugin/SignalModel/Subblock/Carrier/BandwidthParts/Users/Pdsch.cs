using System;
using System.Xml.Linq;
using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using NationalInstruments.Utilities.SignalCreator.RfwsParser.Converters;
    using SignalCreator.RfwsParser;
    using System.Collections.Generic;
    public class PdschSettings
    {
        [RfwsParseableKey("Count", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.NumberOfPdschConfigurations)]
        public int? NumberOfPdsch;

        [RfwsSection(@"PDSCH Slot Settings \d+", version = "4", regExMatch = true)]
        [RFmxMappableSection(SelectorStrings.Pdsch)]
        public List<Pdsch> PdschConfigurations;
    }

    public class Pdsch
    {
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschNumberOfResourceBlockClusters)]
        public int? NumberOfRbClusters => PdschRbClusters?.Count;

        #region Section Version 3 (pre-20.0 release)
        [RfwsParseableKey("RB Allocation", 3, ConverterType = typeof(RbClusterConverter<PdschRbCluster>))]
        [RFmxMappableSection(SelectorStrings.PdschCluster)]
        public List<PdschRbCluster> PdschRbClusters;

        [RfwsParseableKey("PDSCH Present in SSB RB", 3)]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschPresentInSsbResourceBlock)]
        public bool? PdschPressentInSsbRb;
        [RfwsParseableKey("Slot Allocation", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschSlotAllocation)]
        public string SlotAllocation;
        [RfwsParseableKey("Symbol Allocation", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschSymbolAllocation)]
        public string SymbolAllocation;
        [RfwsParseableKey("Modulation Type", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschModulationType)]
        public RFmxNRMXPdschModulationType? ModulationType;
        [RfwsParseableKey("PDSCH Mapping Type", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschMappingType)]
        public RFmxNRMXPdschMappingType? MappingType;
        [RfwsParseableKey("DMRS Duration", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschDmrsDuration)]
        public RFmxNRMXPdschDmrsDuration? DmrsDuration;
        [RfwsParseableKey("DMRS Configuration", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschDmrsConfigurationType)]
        public RFmxNRMXPdschDmrsConfigurationType? DmrsConfiguration;
        [RfwsParseableKey("DMRS Power Mode", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschDmrsPowerMode)]
        public RFmxNRMXPdschDmrsPowerMode? DmrsPowerMode;
        [RfwsParseableKey("DMRS Scaling Factor", 3, ConverterType = typeof(LinearTodBConverter))]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschDmrsPower)]
        public double? DmrsPower;
        [RfwsParseableKey("DMRS Additional Positions", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschDmrsAdditionalPositions)]
        public int? AdditionalPositions;
        [RfwsParseableKey("DMRS Type A Position", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschDmrsTypeAPosition)]
        public int? TypeAPosition;
        [RfwsParseableKey("DMRS Scrambling ID", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschDmrsScramblingID)]
        public int? ScramblingId;
        [RfwsParseableKey("DMRS Scrambling ID Mode", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschDmrsScramblingIDMode)]
        public RFmxNRMXPdschDmrsScramblingIDMode? ScramblingMode;
        [RfwsParseableKey("Number of CDM Groups", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschDmrsNumberOfCdmGroups)]
        public int? CdmGroups;
        [RfwsParseableKey("nSCID", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschDmrsnScid)]
        public int? Nscid;

        [RfwsParseableKey("PTRS Ports", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschPtrsAntennaPorts)]
        public string PtrsPorts;
        [RfwsParseableKey("PTRS Time Density", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschPtrsTimeDensity)]
        public int? PtrsTimeDensity;
        [RfwsParseableKey("PTRS Frequency Density", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschPtrsFrequencyDensity)]
        public int? PtrsFrequencyDensity;
        [RfwsParseableKey("DL PTRS RE Offset", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschPtrsREOffset)]
        public int? PtrsReOffset;
        [RfwsParseableKey("PTRS Enabled", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschPtrsEnabled)]
        public bool? PtrsEnabled;
        [RfwsParseableKey("PTRS Power Mode", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschPtrsPowerMode)]
        public RFmxNRMXPdschPtrsPowerMode? PtrsPowerMode;
        [RfwsParseableKey("PTRS Scaling Factor", 3, ConverterType = typeof(LinearTodBConverter))]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschPtrsPower)]
        public double? PtrsPower;
        [RfwsParseableKey("DMRS Ports", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschDmrsAntennaPorts)]
        public string DmrsPorts;
        #endregion

        #region Section Version 4 (20.0 Release and Above)
        public class PdschDrmsReleaseVersionConverter : EnumConverter<RFmxNRMXPdschDmrsReleaseVersion>
        {
            protected override RFmxNRMXPdschDmrsReleaseVersion Convert(object value)
            {
                string textValue = (string)value;
                textValue = textValue.Replace("3GPP", string.Empty);
                return base.Convert(textValue);
            }
        }

        [RfwsParseableKey("Dmrs Release Version", 4, ConverterType = typeof(PdschDrmsReleaseVersionConverter))]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.PdschDmrsReleaseVersion)]
        public RFmxNRMXPdschDmrsReleaseVersion? ReleaseVersion;
        #endregion
    }



}
