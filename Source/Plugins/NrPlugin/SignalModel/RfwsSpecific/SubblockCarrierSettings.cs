using System;
using System.Xml.Linq;
using System.Collections.Generic;
using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using SignalCreator.RfwsParser;

    [RfwsSection("Carrier", version = "3")]
    public class SubblockCarrierSettings
    {
        static double absoluteFrequency = 1e9;

        [RfwsParseableKey("CarrierDefinition", 1)]
        public int? CarrierDefinitionIndex;
        [RfwsParseableKey("CarrierSubblockNumber", 1)]
        public int? SubblockIndex;
        [RfwsParseableKey("CarrierCCIndex", 1)]
        public int? ComponentCarrierIndex;

        private class FrequencyConverter : ValueConverter<double>
        {
            protected override double Convert(object value)
            {
                return base.Convert(value) + absoluteFrequency;
            }
        }

        #region RFmx Properties
        [RfwsParseableKey("CarrierSubblockOffset", 3, ConverterType = typeof(FrequencyConverter))]
        public double? CarrierSubblockOffset;

        [RfwsParseableKey("CarrierFrequencyOffset", 3)]
        public double? CarrierFrequencyOffset;
        #endregion
    }
}
