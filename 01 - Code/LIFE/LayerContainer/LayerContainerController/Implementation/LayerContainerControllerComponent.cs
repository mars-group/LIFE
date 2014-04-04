using Hik.Communication.ScsServices.Service;
using LayerContainerController.Interfaces;
using LCConnector;
using LCConnector.TransportTypes;
using NodeRegistry.Interface;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;

namespace LayerContainerController.Implementation {
    public class LayerContainerControllerComponent : ILayerContainerController  {
        
        private ILayerContainerController _layerContainerControllerUseCase;

        public LayerContainerControllerComponent(
            IPartitionManager partitionManager,
            IRTEManager rteManager,
            INodeRegistry nodeRegistry) {
            _layerContainerControllerUseCase = new LayerContainerControllerUseCase(partitionManager, rteManager,
                nodeRegistry);
        }


        public void Instantiate(TLayerInstanceId instanceId) {
            _layerContainerControllerUseCase.Instantiate(instanceId);
        }

        public void InitializeLayer(TLayerInstanceId instanceId, TInitData initData) {
            _layerContainerControllerUseCase.InitializeLayer(instanceId, initData);
        }

        public long Tick() {
            return _layerContainerControllerUseCase.Tick();
        }
    }
}