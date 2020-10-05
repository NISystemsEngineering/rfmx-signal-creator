using System;
using System.Xml.Linq;
using NationalInstruments.RFmx.NRMX;


namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using NationalInstruments.Utilities.SignalCreator;
    using SignalCreator.RfwsParser;

    public class OutputSettings
    {
        [RfwsParseableKey("Link Direction", 3)]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.LinkDirection, RfmxNrSelectorStringType.None)]
        public RFmxNRMXLinkDirection? LinkDirection;
        [RfwsParseableKey("DL Ch Configuration Mode", 3)]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.DownlinkChannelConfigurationMode, RfmxNrSelectorStringType.None)]
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
        [RfwsParseableKey("DL Test Model", 3, ConverterType = typeof(TestModelConverter))]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.DownlinkTestModel, RfmxNrSelectorStringType.None)]
        public RFmxNRMXDownlinkTestModel? DlTestModel;
        [RfwsParseableKey("DL Test Model Duplex Scheme", 3)]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.DownlinkTestModelDuplexScheme, SelectorStringType = RfmxNrSelectorStringType.None)]
        public RFmxNRMXDownlinkTestModelDuplexScheme? DlTestModelDuplexScheme;
    }
}
