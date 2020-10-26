using NationalInstruments.RFmx.NRMX;
using System.Collections.Generic;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using Serialization;

    internal class BandwidthPartSettings
    {
        [RfwsDeserializableKey("Bandwidth Part Index", 3)]
        public int? BandwidthPartIndex;

        #region RFmx Properties
        [RfwsDeserializableKey("Subcarrier Spacing (Hz)", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.BandwidthPartSubcarrierSpacing)]
        public double? SubcarrierSpacing;
        [RfwsDeserializableKey("Cyclic Prefix Mode", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.BandwidthPartCyclicPrefixMode)]
        public RFmxNRMXBandwidthPartCyclicPrefixMode? CyclicPrefixMode;
        [RfwsDeserializableKey("Grid Start", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.GridStart)]
        public int? GridStart;
        [RfwsDeserializableKey("RB Offset", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.BandwidthPartResourceBlockOffset)]
        public int? RbOffset;
        [RfwsDeserializableKey("Number of RBs", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.BandwidthPartNumberOfResourceBlocks)]
        public int? NumberOfRbs;
        [RfwsDeserializableKey("UE Count", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.NumberOfUsers)]
        public int? NumberOfUsers;
        [RfwsDeserializableKey("Coreset Count", 3), RFmxNrSerializableProperty(RFmxNRMXPropertyId.NumberOfCoresets)]
        public int? NumberOfCoresets;
        #endregion

        [RfwsDeserializableSection(@"UE Settings \d+", 1, regExMatch = true)]
        [RFmxSerializableSection(SelectorStrings.User)]
        public List<User> Users;
        [RfwsDeserializableSection(@"CORESET Settings \d+", 1, regExMatch = true)]
        [RFmxSerializableSection(SelectorStrings.Coreset)]
        public List<CoresetSettings> Coresets;

    }
}
