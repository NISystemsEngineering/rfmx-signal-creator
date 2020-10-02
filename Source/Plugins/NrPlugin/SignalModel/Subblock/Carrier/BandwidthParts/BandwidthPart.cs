using NationalInstruments.RFmx.NRMX;
using System.Collections.Generic;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using SignalCreator.RfwsParser;

    // Bandwidth Part Section has number in title
    [RfwsSection(@"Bandwidth Part Settings \d+", version = "3", regExMatch = true)]
    public class BandwidthPartSettings
    {
        [RfwsParseableKey("Bandwidth Part Index", 3)]
        public int? BandwidthPartIndex;

        #region RFmx Properties
        [RfwsParseableKey("Subcarrier Spacing (Hz)", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.BandwidthPartSubcarrierSpacing)]
        public double? SubcarrierSpacing;
        [RfwsParseableKey("Cyclic Prefix Mode", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.BandwidthPartCyclicPrefixMode)]
        public RFmxNRMXBandwidthPartCyclicPrefixMode? CyclicPrefixMode;
        [RfwsParseableKey("Grid Start", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.GridStart)]
        public int? GridStart;
        [RfwsParseableKey("RB Offset", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.BandwidthPartResourceBlockOffset)]
        public int? RbOffset;
        [RfwsParseableKey("Number of RBs", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.BandwidthPartNumberOfResourceBlocks)]
        public int? NumberOfRbs;
        [RfwsParseableKey("UE Count", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.NumberOfUsers)]
        public int? NumberOfUsers;
        [RfwsParseableKey("Coreset Count", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.NumberOfCoresets)]
        public int? NumberOfCoresets;
        #endregion

        [RfwsSection(@"UE Settings \d+", version = "1", regExMatch = true)]
        [RFmxMappableSection(SelectorStrings.User)]
        public List<User> Users;
        [RfwsSection(@"CORESET Settings \d+", version = "1", regExMatch = true)]
        [RFmxMappableSection(SelectorStrings.Coreset)]
        public List<CoresetSettings> Coresets;

    }
}
