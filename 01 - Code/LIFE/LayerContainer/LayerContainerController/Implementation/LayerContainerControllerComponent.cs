
using LayerContainerController.Interfaces;
using LCConnector.TransportTypes.ModelStructure;
using NodeRegistry.Interface;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;
using LCConnector.TransportTypes;


namespace LayerContainerController.Implementation {


    public class LayerContainerControllerComponent : ILayerContainerController  {
        
        private readonly ILayerContainerController _layerContainerControllerUseCase;

        public LayerContainerControllerComponent(
            IPartitionManager partitionManager,
            IRTEManager rteManager) {
            _layerContainerControllerUseCase = new LayerContainerControllerUseCase(partitionManager, rteManager);
        }

        public void LoadModelContent(ModelContent content) {
            _layerContainerControllerUseCase.LoadModelContent(content);
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