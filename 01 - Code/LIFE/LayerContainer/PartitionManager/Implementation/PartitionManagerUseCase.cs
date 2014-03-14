using CommonTypes.DataTypes;
using LayerRegistry.Interfaces;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;

namespace PartitionManager.Implementation
{
    class PartitionManagerUseCase : IPartitionManager
    {
        private readonly ILayerRegistry _layerRegistry;

        public PartitionManagerUseCase(ILayerRegistry layerRegistry, IRTEManager rteManager)
        {
            _layerRegistry = layerRegistry;
        }

        public void Setup(DistributionInformation distributionInformation)
        {
            
        }
    }
}
