using NationalInstruments.RFmx.NRMX;


namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using Serialization;
    using Serialization.Converters;

    internal class OutputSettings
    {
        [RfwsDeserializableKey("Link Direction", 3)]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.LinkDirection, RfmxNrSelectorStringType.None)]
        public RFmxNRMXLinkDirection? LinkDirection;
        [RfwsDeserializableKey("DL Ch Configuration Mode", 3)]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.DownlinkChannelConfigurationMode, RfmxNrSelectorStringType.None)]
        public RFmxNRMXDownlinkChannelConfigurationMode? DlChannelConfigMode;

        class TestModelConverter : EnumConverter<RFmxNRMXDownlinkTestModel>
        {
            public override StripOptions CharacterStripOptions => StripOptions.Whitespace;
            protected override RFmxNRMXDownlinkTestModel Convert(object value)
            {
                string textValue = (string)value;
                textValue = textValue.Replace(".", "_");
                return base.Convert(textValue);
            }
        }
        [RfwsDeserializableKey("DL Test Model", 3, ConverterType = typeof(TestModelConverter))]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.DownlinkTestModel, RfmxNrSelectorStringType.None)]
        public RFmxNRMXDownlinkTestModel? DlTestModel;
        [RfwsDeserializableKey("DL Test Model Duplex Scheme", 3)]
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.DownlinkTestModelDuplexScheme, SelectorStringType = RfmxNrSelectorStringType.None)]
        public RFmxNRMXDownlinkTestModelDuplexScheme? DlTestModelDuplexScheme;
    }
}
