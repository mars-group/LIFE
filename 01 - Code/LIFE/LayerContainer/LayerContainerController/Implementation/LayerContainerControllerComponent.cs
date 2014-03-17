namespace LayerContainerController.Implementation
{
    using NodeRegistry.Interface;

    using PartitionManager.Interfaces;

    using RTEManager.Interfaces;

    public class LayerContainerControllerComponent
    {
        private LayerContainerControllerUseCase _layerContainerControllerUseCase;

        public LayerContainerControllerComponent(
            IPartitionManager partitionManager,
            IRTEManager rteManager,
            INodeRegistry nodeRegistry)
        {
            _layerContainerControllerUseCase = new LayerContainerControllerUseCase(partitionManager, rteManager, nodeRegistry);
        }
    }

}
