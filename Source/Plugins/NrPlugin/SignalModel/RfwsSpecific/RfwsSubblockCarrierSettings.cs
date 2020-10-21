namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using Serialization;
    using Serialization.Converters;

    internal class RfwsSubblockCarrierSettings
    {
        static double absoluteFrequency = 1e9;

        [RfwsDeserializableKey("CarrierDefinition", 1)]
        public int? CarrierDefinitionIndex;
        [RfwsDeserializableKey("CarrierSubblockNumber", 1)]
        public int? SubblockIndex;
        [RfwsDeserializableKey("CarrierCCIndex", 1)]
        public int? ComponentCarrierIndex;

        private class FrequencyConverter : ValueConverter<double>
        {
            protected override double Convert(object value)
            {
                return base.Convert(value) + absoluteFrequency;
            }
        }

        #region RFmx Properties
        [RfwsDeserializableKey("CarrierSubblockOffset", 3, ConverterType = typeof(FrequencyConverter))]
        public double? CarrierSubblockOffset;

        [RfwsDeserializableKey("CarrierFrequencyOffset", 3)]
        public double? CarrierFrequencyOffset;
        #endregion
    }
}
