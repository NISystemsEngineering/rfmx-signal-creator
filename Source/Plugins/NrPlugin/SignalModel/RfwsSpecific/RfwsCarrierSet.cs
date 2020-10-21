using System.Collections.Generic;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using Serialization;

    /// <summary>
    /// Represents a carrier set configuration within the RFWS file. This does not directly translate to RFmx,
    /// so this is imported as part of the construction of the <see cref="NrSignalModel"/> to translate it to the 
    /// appropriate structure.
    /// </summary>
    internal class RfwsCarrierSet
    {
        #region RFmx Properties
        [RfwsDeserializableKey("AutoIncrementCellIdEnabled", 3)]
        public bool? AutoIncrementCellId;
        #endregion

        #region Sub Sections
        [RfwsDeserializableSection("Carrier", version = "3")]
        public List<RfwsSubblockCarrierSettings> SubblockCarrierSettings;
        #endregion

    }
}
