using Hik.Communication.ScsServices.Service;
using LayerContainerController.Interfaces;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using NodeRegistry.Interface;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;

namespace LayerContainerController.Implementation {
    public class LayerContainerControllerUseCase: ScsService, ILayerContainerController {
        
        private readonly IPartitionManager _partitionManager;

        private readonly IRTEManager _rteManager;

        public LayerContainerControllerUseCase(IPartitionManager partitionManager, IRTEManager rteManager) {
            _partitionManager = partitionManager;
            _rteManager = rteManager;
        }

        public void LoadModelContent(ModelContent content) {
            _partitionManager.LoadModelContent(content);
        }

        public void Instantiate(TLayerInstanceId instanceId) {
            _partitionManager.AddLayer(instanceId);
        }

        public void InitializeLayer(TLayerInstanceId instanceId, TInitData initData) {
            throw new System.NotImplementedException();
        }

        public long Tick() {
            return _rteManager.AdvanceOneTick();
        }
    }
}