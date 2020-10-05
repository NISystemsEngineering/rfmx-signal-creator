using NationalInstruments.RFmx.NRMX;
using System.Collections.Generic;
using System.Linq;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin.SignalModel
{
    public class SignalModel
    {
        public SignalModel() { }
        public SignalModel(CarrierSet set, List<Carrier> carriers)
        {
            int numSubblocks = set.SubblockCarrierSettings.Max(s => (int)s.SubblockIndex) + 1;
            
            AutoIncrementCellId = set.AutoIncrementCellId;
            NumberOfSubblocks = numSubblocks;
            Subblocks = new List<Subblock>(numSubblocks);

            for (int i = 0; i < numSubblocks; i++)
            {
                var currentSubblockSettings = set.SubblockCarrierSettings.Where(s => s.SubblockIndex == i);
                SubblockCarrierSettings settings = currentSubblockSettings.First();

                int numComponentCarriers = currentSubblockSettings.Count();

                Subblock subblock = new Subblock
                {
                    CarrierSubblockOffset = settings.CarrierSubblockOffset,
                    NumberOfComponentCarriers = numComponentCarriers,
                    ComponentCarriers = new List<Carrier>(numComponentCarriers)
                };
                for (int j = 0; j < numComponentCarriers; j++)
                {
                    SubblockCarrierSettings currentCcSettings = currentSubblockSettings.Where(s => s.ComponentCarrierIndex == j).First();
                    Carrier associatedCarrier = carriers[(int)currentCcSettings.CarrierDefinitionIndex].ShallowCopy();

                    associatedCarrier.CarrierFrequencyOffset = currentCcSettings.CarrierFrequencyOffset;
                    subblock.ComponentCarriers.Add(associatedCarrier);
                }
                Subblocks.Add(subblock);
            }
        }

        [RFmxNrMappableProperty(RFmxNRMXPropertyId.AutoIncrementCellIDEnabled)]
        public bool? AutoIncrementCellId;
        [RFmxNrMappableProperty(RFmxNRMXPropertyId.NumberOfSubblocks)]
        public int NumberOfSubblocks;

        [RFmxMappableSection(SelectorStrings.Subblock)]
        public List<Subblock> Subblocks;

    }
}
