using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using Serialization;
    using Serialization.Converters;

    internal class SsbSettings
    {
        // Using this key as a proxy for SSB enabled. RFmx WC doesn't seem to have a specific property for
        // enabling SSB; instead, this, "SSS Channel Mode", "PBCH DMRS Channel Mode", and 
        // "PBCH Channel Mode" are all set to True when the SSB enabled checkbox is set in the WC UI.
        [RfwsDeserializableKey("PSS Channel Mode", 4), RFmxNrSerializableProperty(RFmxNRMXPropertyId.SsbEnabled)]
        public bool? SsbEnabled;

        class SsbPatternConverter : EnumConverter<RFmxNRMXSsbPattern>
        {
            protected override RFmxNRMXSsbPattern Convert(object value)
            {
                string textValue = (string)value;
                textValue = textValue.Replace("3-up", "3GHz");
                return base.Convert(textValue);
            }
        }
        [RfwsDeserializableKey("Configuration Set", 4, ConverterType = typeof(SsbPatternConverter))]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.SsbPattern)]
        public RFmxNRMXSsbPattern? SsbPattern;
        [RfwsDeserializableKey("SSS Scaling Factor", 4, ConverterType = typeof(LinearTodBConverter))]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.SssPower)]
        public double? SssPower;
        [RfwsDeserializableKey("PSS Scaling Factor", 4, ConverterType = typeof(LinearTodBConverter))]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.PssPower)]
        public double? PssPower;
        [RfwsDeserializableKey("PBCH Scaling Factor", 4, ConverterType = typeof(LinearTodBConverter))]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.PbchPower)]
        public double? PbchPower;
        [RfwsDeserializableKey("PBCH DMRS Scaling Factor", 4, ConverterType = typeof(LinearTodBConverter))]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.PbchDmrsPower)]
        public double? PbchDrmsPower;
        [RfwsDeserializableKey("Subcarrier Spacing Common", 4), RFmxNrSerializableProperty(RFmxNRMXPropertyId.SubcarrierSpacingCommon)]
        public double? SubcarrierSpacingCommon;
        [RfwsDeserializableKey("Subcarrier Offset", 4)]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.SsbSubcarrierOffset)]
        public double? SubcarrierOffset;
        [RfwsDeserializableKey("Periodicity", 4), RFmxNrSerializableProperty(RFmxNRMXPropertyId.SsbPeriodicity)]
        public double? SsbPeriodicity;
        [RfwsDeserializableKey("SSB Active Blocks", 4), RFmxNrSerializableProperty(RFmxNRMXPropertyId.SsbActiveBlocks)]
        public string SsbActiveBlocks;
    }
}
