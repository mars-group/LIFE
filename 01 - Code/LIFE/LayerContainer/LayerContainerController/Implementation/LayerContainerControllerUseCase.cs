namespace LayerContainerController.Implementation
{
    using NodeRegistry.Interface;
    using PartitionManager.Interfaces;
    using RTEManager.Interfaces;

    public class LayerContainerControllerUseCase
    {
        private readonly IPartitionManager partitionManager;

        private readonly IRTEManager rteManager;

        private readonly INodeRegistry nodeRegistry;

        public LayerContainerControllerUseCase(IPartitionManager partitionManager, IRTEManager rteManager, INodeRegistry nodeRegistry)
        {
            this.partitionManager = partitionManager;
            this.rteManager = rteManager;
            this.nodeRegistry = nodeRegistry;
        }
    }
}
