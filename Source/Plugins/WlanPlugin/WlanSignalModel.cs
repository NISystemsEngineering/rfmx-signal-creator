using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.RFmx.WlanMX;
using NationalInstruments.RFToolkits.Interop;

namespace NationalInstruments.Utilities.SignalCreator.Plugins
{

    class WlanSignalGroup : PropertyGroup
    {
        public WlanSignalGroup(string selectorString)
            : base(selectorString) { }

        [WlanProperty(niWLANGProperties.ChannelBandwidth)]
        public PropertyMap<double> ChannelBandwidth = new PropertyMap<double>
        {
            RfmxPropertyId = (int)RFmxWlanMXPropertyId.ChannelBandwidth,
        };

        private static readonly Dictionary<int, RFmxWlanMXStandard> standardMap = new Dictionary<int, RFmxWlanMXStandard>
        {
            [niWLANGConstants.Standard80211AcMimoOfdm] = RFmxWlanMXStandard.Standard802_11ac,
            [niWLANGConstants.Standard80211agOfdm] = RFmxWlanMXStandard.Standard802_11ag,
            [niWLANGConstants.Standard80211AxMimoOfdm] = RFmxWlanMXStandard.Standard802_11ax,
            [niWLANGConstants.Standard80211bgDsss] = RFmxWlanMXStandard.Standard802_11b,
            [niWLANGConstants.Standard80211jOfdm] = RFmxWlanMXStandard.Standard802_11j,
            [niWLANGConstants.Standard80211nMimoOfdm] = RFmxWlanMXStandard.Standard802_11n,
            [niWLANGConstants.Standard80211pOfdm] = RFmxWlanMXStandard.Standard802_11p
        };
        [WlanProperty(niWLANGProperties.Standard)]
        public PropertyMap<int> Standard = new PropertyMap<int>
        {
            RfmxPropertyId = (int)RFmxWlanMXPropertyId.Standard,
            CustomMap = (value) =>
            {
                int standardValue = (int)value;
                bool result = standardMap.TryGetValue(standardValue, out RFmxWlanMXStandard standard);
                if (!result) throw new NotSupportedException($"Waveform standard type is not supported by RFmx WLAN.");
                return (int)standard;
            }
        };
    }
}
