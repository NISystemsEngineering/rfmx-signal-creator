using NationalInstruments.RFmx.NRMX;
using System.Collections.Generic;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using Serialization;
    using Serialization.Converters;

    internal class PdschSettings
    {
        [RfwsDeserializableKey("Count", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.NumberOfPdschConfigurations)]
        public int? NumberOfPdsch;

        [RfwsDeserializableSection(@"PDSCH Slot Settings \d+", version = "4", regExMatch = true)]
        [RFmxSerializableSection(SelectorStrings.Pdsch)]
        public List<Pdsch> PdschConfigurations;
    }

    internal class Pdsch
    {
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschNumberOfResourceBlockClusters)]
        public int? NumberOfRbClusters => PdschRbClusters?.Count;

        #region Section Version 3 (pre-20.0 release)
        [RfwsDeserializableKey("RB Allocation", 3, ConverterType = typeof(RbClusterConverter<PdschRbCluster>))]
        [RFmxSerializableSection(SelectorStrings.PdschCluster)]
        public List<PdschRbCluster> PdschRbClusters;

        [RfwsDeserializableKey("PDSCH Present in SSB RB", 3)]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschPresentInSsbResourceBlock)]
        public bool? PdschPressentInSsbRb;
        [RfwsDeserializableKey("Slot Allocation", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschSlotAllocation)]
        public string SlotAllocation;
        [RfwsDeserializableKey("Symbol Allocation", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschSymbolAllocation)]
        public string SymbolAllocation;
        [RfwsDeserializableKey("Modulation Type", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschModulationType)]
        public RFmxNRMXPdschModulationType? ModulationType;
        [RfwsDeserializableKey("PDSCH Mapping Type", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschMappingType)]
        public RFmxNRMXPdschMappingType? MappingType;
        [RfwsDeserializableKey("DMRS Duration", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschDmrsDuration)]
        public RFmxNRMXPdschDmrsDuration? DmrsDuration;
        [RfwsDeserializableKey("DMRS Configuration", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschDmrsConfigurationType)]
        public RFmxNRMXPdschDmrsConfigurationType? DmrsConfiguration;
        [RfwsDeserializableKey("DMRS Power Mode", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschDmrsPowerMode)]
        public RFmxNRMXPdschDmrsPowerMode? DmrsPowerMode;
        [RfwsDeserializableKey("DMRS Scaling Factor", 3, ConverterType = typeof(LinearTodBConverter))]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschDmrsPower)]
        public double? DmrsPower;
        [RfwsDeserializableKey("DMRS Additional Positions", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschDmrsAdditionalPositions)]
        public int? AdditionalPositions;
        [RfwsDeserializableKey("DMRS Type A Position", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschDmrsTypeAPosition)]
        public int? TypeAPosition;
        [RfwsDeserializableKey("DMRS Scrambling ID", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschDmrsScramblingID)]
        public int? ScramblingId;
        [RfwsDeserializableKey("DMRS Scrambling ID Mode", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschDmrsScramblingIDMode)]
        public RFmxNRMXPdschDmrsScramblingIDMode? ScramblingMode;
        [RfwsDeserializableKey("Number of CDM Groups", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschDmrsNumberOfCdmGroups)]
        public int? CdmGroups;
        [RfwsDeserializableKey("nSCID", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschDmrsnScid)]
        public int? Nscid;

        [RfwsDeserializableKey("PTRS Ports", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschPtrsAntennaPorts)]
        public string PtrsPorts;
        [RfwsDeserializableKey("PTRS Time Density", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschPtrsTimeDensity)]
        public int? PtrsTimeDensity;
        [RfwsDeserializableKey("PTRS Frequency Density", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschPtrsFrequencyDensity)]
        public int? PtrsFrequencyDensity;
        [RfwsDeserializableKey("DL PTRS RE Offset", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschPtrsREOffset)]
        public int? PtrsReOffset;
        [RfwsDeserializableKey("PTRS Enabled", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschPtrsEnabled)]
        public bool? PtrsEnabled;
        [RfwsDeserializableKey("PTRS Power Mode", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschPtrsPowerMode)]
        public RFmxNRMXPdschPtrsPowerMode? PtrsPowerMode;
        [RfwsDeserializableKey("PTRS Scaling Factor", 3, ConverterType = typeof(LinearTodBConverter))]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschPtrsPower)]
        public double? PtrsPower;
        [RfwsDeserializableKey("DMRS Ports", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschDmrsAntennaPorts)]
        public string DmrsPorts;
        #endregion

        #region Section Version 4 (20.0 Release and Above)
        internal class PdschDrmsReleaseVersionConverter : EnumConverter<RFmxNRMXPdschDmrsReleaseVersion>
        {
            protected override RFmxNRMXPdschDmrsReleaseVersion Convert(object value)
            {
                string textValue = (string)value;
                textValue = textValue.Replace("3GPP", string.Empty);
                return base.Convert(textValue);
            }
        }

        [RfwsDeserializableKey("Dmrs Release Version", 4, ConverterType = typeof(PdschDrmsReleaseVersionConverter))]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.PdschDmrsReleaseVersion)]
        public RFmxNRMXPdschDmrsReleaseVersion? ReleaseVersion;
        #endregion
    }



}
