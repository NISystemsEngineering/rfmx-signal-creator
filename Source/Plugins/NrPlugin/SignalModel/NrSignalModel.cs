using NationalInstruments.RFmx.NRMX;
using System.Collections.Generic;
using System.Linq;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    using Serialization;

    /// <summary>
    /// Represents the 5G NR signal according to a merged layout of the RFWS file and RFmx tree structure.
    /// </summary>
    internal class NrSignalModel
    {
        /// <summary>
        /// Constructs the unified signal model from an individual RFWS carrier set definition and the list of carriers
        /// that are included within that carrier set definition.
        /// </summary>
        /// <param name="set"></param>
        /// <param name="carriers"></param>
        public NrSignalModel(RfwsCarrierSet set, List<Carrier> carriers)
        {
            int numSubblocks = set.SubblockCarrierSettings.Max(s => (int)s.SubblockIndex) + 1;
            
            AutoIncrementCellId = set.AutoIncrementCellId;
            NumberOfSubblocks = numSubblocks;
            Subblocks = new List<Subblock>(numSubblocks);

            for (int i = 0; i < numSubblocks; i++)
            {
                // Get all component carriers for the current subblock iteration
                var currentSubblockSettings = set.SubblockCarrierSettings.Where(s => s.SubblockIndex == i);
                // The subblock properties are identical in each subblock configuration, so using the first is fine
                RfwsSubblockCarrierSettings settings = currentSubblockSettings.First();

                // Since the RFWS file uses a flattened structure, there will be a separate subblock configuraiton object
                // for each component carriers. Since we have already filtered the list to select the currrent subblock,
                // the number of items now represents the number of component carriers within this subblock.
                int numComponentCarriers = currentSubblockSettings.Count();

                Subblock subblock = new Subblock
                {
                    CarrierSubblockOffset = settings.CarrierSubblockOffset,
                    NumberOfComponentCarriers = numComponentCarriers,
                    ComponentCarriers = new List<Carrier>(numComponentCarriers)
                };
                for (int j = 0; j < numComponentCarriers; j++)
                {
                    // Load the specific settings for the current component carrier that is being configured
                    RfwsSubblockCarrierSettings currentCcSettings = currentSubblockSettings.Where(s => s.ComponentCarrierIndex == j).First();
                    // The object contains a CarrierDefinitionIndex field that describes which carrier definition it is using.
                    // Since these may be used multiple times and certain properties may be changed depending on the configuration,
                    // we clone the object so that we have a unique instance.
                    Carrier associatedCarrier = carriers[(int)currentCcSettings.CarrierDefinitionIndex].ShallowCopy();

                    associatedCarrier.CarrierFrequencyOffset = currentCcSettings.CarrierFrequencyOffset;
                    subblock.ComponentCarriers.Add(associatedCarrier);
                }
                Subblocks.Add(subblock);
            }
        }

        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.AutoIncrementCellIDEnabled)]
        public bool? AutoIncrementCellId;
        [RFmxNrSerializableProperty(RFmxNRMXPropertyId.NumberOfSubblocks)]
        public int NumberOfSubblocks;

        [RFmxSerializableSection(SelectorStrings.Subblock)]
        public List<Subblock> Subblocks;

    }
}
