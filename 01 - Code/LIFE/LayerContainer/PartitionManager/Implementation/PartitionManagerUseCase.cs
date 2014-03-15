using CommonTypes.DataTypes;
using LayerRegistry.Interfaces;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;

namespace PartitionManager.Implementation
{
    class PartitionManagerUseCase : IPartitionManager
    {
        private readonly ILayerRegistry _layerRegistry;

        private readonly IRTEManager _rteManager;

        public PartitionManagerUseCase(ILayerRegistry layerRegistry, IRTEManager rteManager)
        {
            _layerRegistry = layerRegistry;
            _rteManager = rteManager;
        }

        public void Setup(DistributionInformation distributionInformation)
        {
            
        }
    }
}
