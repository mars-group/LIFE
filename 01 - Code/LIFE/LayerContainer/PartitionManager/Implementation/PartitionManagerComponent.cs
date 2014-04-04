using System;
using CommonTypes.TransportTypes.SimulationControl;
using LayerRegistry.Interfaces;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;

namespace PartitionManager.Implementation {
    public class PartitionManagerComponent : IPartitionManager {
        private readonly PartitionManagerUseCase _partitionManagerUseCase;

        public PartitionManagerComponent(ILayerRegistry layerRegistry, IRTEManager rteManager) {
            _partitionManagerUseCase = new PartitionManagerUseCase(layerRegistry, rteManager);
        }

        public void Setup(DistributionInformation distributionInformation) {
            _partitionManagerUseCase.Setup(distributionInformation);
        }

        public bool AddLayer(Uri layerUri, Guid layerID) {
            return _partitionManagerUseCase.AddLayer(layerUri, layerID);
        }
    }
}