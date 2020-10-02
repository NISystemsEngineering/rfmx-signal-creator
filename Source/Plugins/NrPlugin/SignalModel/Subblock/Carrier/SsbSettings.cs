using System;
using System.Xml.Linq;
using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using RfwsParser.Converters;
    using SignalCreator.RfwsParser;

    public class SsbSettings
    {

        // Using this key as a proxy for SSB enabled. RFmx WC doesn't seem to have a specific property for
        // enabling SSB; instead, this, "SSS Channel Mode", "PBCH DMRS Channel Mode", and 
        // "PBCH Channel Mode" are all set to True when the SSB enabled checkbox is set in the WC UI.
        [RfwsParseableKey("PSS Channel Mode", 4), RFmxNrMappableProperty(RFmxNRMXPropertyId.SsbEnabled)]
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
        [RfwsParseableKey("Configuration Set", 4, ConverterType = typeof(SsbPatternConverter))]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.SsbPattern)]
        public RFmxNRMXSsbPattern? SsbPattern;
        [RfwsParseableKey("SSS Scaling Factor", 4, ConverterType = typeof(LinearTodBConverter))]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.SssPower)]
        public double? SssPower;
        [RfwsParseableKey("PSS Scaling Factor", 4, ConverterType = typeof(LinearTodBConverter))]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.PssPower)]
        public double? PssPower;
        [RfwsParseableKey("PBCH Scaling Factor", 4, ConverterType = typeof(LinearTodBConverter))]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.PbchPower)]
        public double? PbchPower;
        [RfwsParseableKey("PBCH DMRS Scaling Factor", 4, ConverterType = typeof(LinearTodBConverter))]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.PbchDmrsPower)]
        public double? PbchDrmsPower;
        [RfwsParseableKey("Subcarrier Spacing Common", 4), RFmxNrMappableProperty(RFmxNRMXPropertyId.SubcarrierSpacingCommon)]
        public double? SubcarrierSpacingCommon;
        [RfwsParseableKey("Subcarrier Offset", 4)]
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.SsbSubcarrierOffset)]
        public double? SubcarrierOffset;
        [RfwsParseableKey("Periodicity", 4), RFmxNrMappableProperty(RFmxNRMXPropertyId.SsbPeriodicity)]
        public double? SsbPeriodicity;
        [RfwsParseableKey("SSB Active Blocks", 4), RFmxNrMappableProperty(RFmxNRMXPropertyId.SsbActiveBlocks)]
        public string SsbActiveBlocks;
    }
}
