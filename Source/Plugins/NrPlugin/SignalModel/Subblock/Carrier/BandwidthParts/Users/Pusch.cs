using NationalInstruments.RFmx.NRMX;
using System.Collections.Generic;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using Serialization;
    using Serialization.Converters;

    internal class PuschSettings
    {
        [RfwsDeserializableKey("Count", 1), RFmxNrSerializableProperty(RFmxNRMXPropertyId.NumberOfPuschConfigurations)]
        public int? NumPusch;

        [RfwsDeserializableSection(@"PUSCH Slot Settings \d+", 6, regExMatch = true)]
        [RFmxSerializableSection(SelectorStrings.Pusch)]
        public List<Pusch> PuschConfigurations;
    }

    internal class Pusch 
    {

        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschNumberOfResourceBlockClusters)]
        public int? NumberOfRbClusters => PuschRbClusters?.Count;

        #region Section Version 3 (Pre 20.0 release and above)
        [RfwsDeserializableKey("RB Allocation", 3, ConverterType = typeof(RbClusterConverter<PuschRbCluster>))]
        [RFmxSerializableSection(SelectorStrings.PuschCluster)]
        public List<PuschRbCluster> PuschRbClusters;
        [RfwsDeserializableKey("Slot Allocation", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschSlotAllocation)]
        public string RbAllocation;
        [RfwsDeserializableKey("Symbol Allocation", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschSymbolAllocation)]
        public string SymbolAllocation;
        [RfwsDeserializableKey("Modulation Type", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschModulationType)]
        public RFmxNRMXPuschModulationType? ModulationType;
        [RfwsDeserializableKey("PUSCH Mapping Type", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschMappingType)]
        public RFmxNRMXPuschMappingType? MappingType;
        [RfwsDeserializableKey("DMRS Duration", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschDmrsDuration)]
        public RFmxNRMXPuschDmrsDuration? DmrsDuration;
        [RfwsDeserializableKey("DMRS Configuration Type", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschDmrsConfigurationType)]
        public RFmxNRMXPuschDmrsConfigurationType? DmrsConfiguration;
        [RfwsDeserializableKey("DMRS Power Mode", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschDmrsPowerMode)]
        public RFmxNRMXPuschDmrsPowerMode? DmrsPowerMode;
        [RfwsDeserializableKey("DMRS Scaling Factor", 3, ConverterType = typeof(LinearTodBConverter))]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschDmrsPower)]
        public double? DmrsPower;
        [RfwsDeserializableKey("DMRS Additional Positions", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschDmrsAdditionalPositions)]
        public int? AdditionalPositions;
        [RfwsDeserializableKey("DMRS Type A Position", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschDmrsTypeAPosition)]
        public int? TypeAPosition;
        [RfwsDeserializableKey("Transform Precoding Enabled", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschTransformPrecodingEnabled)]
        public bool? TransformPreCodingEnabled;
        [RfwsDeserializableKey("PTRS Time Density", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschPtrsTimeDensity)]
        public int? PtrsTimeDensity;
        [RfwsDeserializableKey("PTRS Frequency Density", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschPtrsFrequencyDensity)]
        public int? PtrsFrequencyDensity;
        [RfwsDeserializableKey("UL PTRS RE Offset", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschPtrsREOffset)]
        public int? PtrsReOffset;
        [RfwsDeserializableKey("PTRS Enabled", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschPtrsEnabled)]
        public bool? PtrsEnabled;
        [RfwsDeserializableKey("DMRS Scrambling ID", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschDmrsScramblingID)]
        public int? ScramblingId;
        [RfwsDeserializableKey("DMRS Scrambling ID Mode", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschDmrsScramblingIDMode)]
        public RFmxNRMXPuschDmrsScramblingIDMode? ScramblingMode;
        // PUSCH DMRS Release Version
        [RfwsDeserializableKey("PTRS Power Mode", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschPtrsPowerMode)]
        public RFmxNRMXPuschPtrsPowerMode? PtrsPowerMode;
        [RfwsDeserializableKey("PTRS Scaling Factor", 3, ConverterType = typeof(LinearTodBConverter))]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschPtrsPower)]
        public double? PtrsPower;
        [RfwsDeserializableKey("PUSCH ID", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschDmrsPuschID)]
        public int? DrmsPuschId;
        [RfwsDeserializableKey("PUSCH ID Mode", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschDmrsPuschIDMode)]
        public RFmxNRMXPuschDmrsPuschIDMode? DrmsPuschIdMode;
        [RfwsDeserializableKey("Number of CDM Groups", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschDmrsNumberOfCdmGroups)]
        public int? CdmGroups;
        [RfwsDeserializableKey("DMRS Ports", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschDmrsAntennaPorts)]
        public string DmrsPorts;
        [RfwsDeserializableKey("PTRS Ports", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschPtrsAntennaPorts)]
        public string PtrsPorts;
        #endregion

        #region Section Version 6 (20.0 release)
        internal class PuschDrmsReleaseVersionConverter : EnumConverter<RFmxNRMXPuschDmrsReleaseVersion>
        {
            protected override RFmxNRMXPuschDmrsReleaseVersion Convert(object value)
            {
                string textValue = (string)value;
                textValue = textValue.Replace("3GPP", string.Empty);
                return base.Convert(textValue);
            }
        }

        [RfwsDeserializableKey("Dmrs Release Version", 6, ConverterType = typeof(PuschDrmsReleaseVersionConverter))]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.PuschDmrsReleaseVersion)]
        public RFmxNRMXPuschDmrsReleaseVersion? ReleaseVersion;
        #endregion
    }



}
