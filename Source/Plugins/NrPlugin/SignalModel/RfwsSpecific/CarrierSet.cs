using System;
using System.Xml.Linq;
using NationalInstruments.RFmx.NRMX;

    namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using SignalCreator.RfwsParser;
    using System.Collections.Generic;

    [RfwsSection("CarrierSet", version = "3")]
    public class CarrierSet
    {
        #region RFmx Properties
        [RfwsParseableKey("AutoIncrementCellIdEnabled", 3)]
        public bool? AutoIncrementCellId;
        #endregion

        #region Sub Sections
        [RfwsSectionList("Carrier", version = "3")]
        public List<SubblockCarrierSettings> SubblockCarrierSettings;
        #endregion

    }
}
