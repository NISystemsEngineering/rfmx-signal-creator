using System.Collections.Generic;
using NationalInstruments.RFmx.WlanMX;
using NationalInstruments.RFToolkits.Interop;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.WlanPlugin
{
    using Serialization.Converters;
    internal class WlanSignalGroup
    {
        [WlanTkParseable(niWLANGProperties.ChannelBandwidth)]
        [RFmxWlanSerializableProperty(RFmxWlanMXPropertyId.ChannelBandwidth)]
        public double? ChannelBandwidth;

        private class WlanStandardConverter : LookupTableConverter<int, RFmxWlanMXStandard>
        {
            protected override Dictionary<int, RFmxWlanMXStandard> LookupTable =>
                new Dictionary<int, RFmxWlanMXStandard>
            {
                [niWLANGConstants.Standard80211AcMimoOfdm] = RFmxWlanMXStandard.Standard802_11ac,
                [niWLANGConstants.Standard80211agOfdm] = RFmxWlanMXStandard.Standard802_11ag,
                [niWLANGConstants.Standard80211AxMimoOfdm] = RFmxWlanMXStandard.Standard802_11ax,
                [niWLANGConstants.Standard80211bgDsss] = RFmxWlanMXStandard.Standard802_11b,
                [niWLANGConstants.Standard80211jOfdm] = RFmxWlanMXStandard.Standard802_11j,
                [niWLANGConstants.Standard80211nMimoOfdm] = RFmxWlanMXStandard.Standard802_11n,
                [niWLANGConstants.Standard80211pOfdm] = RFmxWlanMXStandard.Standard802_11p
            };
        }

        [WlanTkParseable(niWLANGProperties.Standard, ConverterType = typeof(WlanStandardConverter))]
        [RFmxWlanSerializableProperty(RFmxWlanMXPropertyId.Standard)]
        public RFmxWlanMXStandard? Standard;

    }
}
