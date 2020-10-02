using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using SignalCreator.RfwsParser;

    public class Pdcch
    {
        #region RFmx Properties
        [RfwsParseableKey("Cce Aggregation Level", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdcchCceAggregationLevel)]
        public int? CceAggregationLevel;
        [RfwsParseableKey("CCE Offset", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdcchCceOffset)]
        public int? CceOffset;
        [RfwsParseableKey("Slot Allocation", 3), RFmxNrMappableProperty(RFmxNRMXPropertyId.PdcchSlotAllocation)]
        public string SlotAllocation;
        #endregion
    }
}
