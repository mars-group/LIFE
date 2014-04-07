
using LayerContainerController.Interfaces;

using NodeRegistry.Interface;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;
using LCConnector.TransportTypes;


namespace LayerContainerController.Implementation {


    public class LayerContainerControllerComponent : ILayerContainerController  {
        
        private readonly ILayerContainerController _layerContainerControllerUseCase;

        public LayerContainerControllerComponent(
            IPartitionManager partitionManager,
            IRTEManager rteManager,
            INodeRegistry nodeRegistry) {
            _layerContainerControllerUseCase = new LayerContainerControllerUseCase(partitionManager, rteManager,
                nodeRegistry);
        }


        public void Instantiate(TLayerInstanceId instanceId, byte[] binaryData) {
            _layerContainerControllerUseCase.Instantiate(instanceId, binaryData);
        }

        public void InitializeLayer(TLayerInstanceId instanceId, TInitData initData) {
            _layerContainerControllerUseCase.InitializeLayer(instanceId, initData);
        }

        public long Tick() {
            return _layerContainerControllerUseCase.Tick();
        }
    }
}