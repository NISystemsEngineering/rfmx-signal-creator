using System;
using System.Xml.Linq;
using NationalInstruments.RFmx.NRMX;
using System.Collections.Generic;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using SignalCreator.RfwsParser;
    // Section has number in title
    public class User
    {
        [RfwsParseableKey("UE Index", 1)]
        public int? UeIndex;
        #region RFmx Properties

        [RfwsParseableKey("nRNTI", 1), RFmxNrMappableProperty(RFmxNRMXPropertyId.Rnti)]
        public int? Rnti;
        #endregion

        [RfwsSection("PDSCH Settings", version = "3"), RFmxMappableSection]
        public PdschSettings PdschConfiguration;

        [RfwsSectionList("PUSCH Settings", version = "1"), RFmxMappableSection]
        public PuschSettings PuschConfiguration;
    }
}
