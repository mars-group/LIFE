using LayerContainerController.Interfaces;
using NodeRegistry.Interface;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;

namespace LayerContainerController.Implementation {
    public class LayerContainerControllerComponent : ILayerContainerController {
        private LayerContainerControllerUseCase _layerContainerControllerUseCase;

        public LayerContainerControllerComponent(
            IPartitionManager partitionManager,
            IRTEManager rteManager,
            INodeRegistry nodeRegistry) {
            _layerContainerControllerUseCase = new LayerContainerControllerUseCase(partitionManager, rteManager,
                nodeRegistry);
        }
    }
}