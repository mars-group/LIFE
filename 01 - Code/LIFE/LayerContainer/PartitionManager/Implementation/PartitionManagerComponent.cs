using CommonTypes.DataTypes;
using LayerRegistry.Interfaces;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;

namespace PartitionManager.Implementation
{
    public class PartitionManagerComponent : IPartitionManager
    {
        private readonly PartitionManagerUseCase _partitionManagerUseCase;

        public PartitionManagerComponent(ILayerRegistry layerRegistry, IRTEManager rteManager)
        {
            _partitionManagerUseCase = new PartitionManagerUseCase(layerRegistry, rteManager);
        }

        public void Setup(DistributionInformation distributionInformation)
        {
            _partitionManagerUseCase.Setup(distributionInformation);
        }
    }
}
