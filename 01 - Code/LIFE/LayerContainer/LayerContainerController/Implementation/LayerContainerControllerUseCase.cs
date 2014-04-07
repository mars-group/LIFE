using Hik.Communication.ScsServices.Service;
using LayerContainerController.Interfaces;
using LCConnector.TransportTypes;
using NodeRegistry.Interface;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;

namespace LayerContainerController.Implementation {
    public class LayerContainerControllerUseCase: ScsService, ILayerContainerController {
        
        private readonly IPartitionManager _partitionManager;

        private readonly IRTEManager _rteManager;

        private readonly INodeRegistry _nodeRegistry;

        public LayerContainerControllerUseCase(IPartitionManager partitionManager, IRTEManager rteManager,
            INodeRegistry nodeRegistry) {
            _partitionManager = partitionManager;
            _rteManager = rteManager;
            _nodeRegistry = nodeRegistry;
        }

        public void Instantiate(TLayerInstanceId instanceId, byte[] binaryData) {
            throw new System.NotImplementedException();
        }

        public void InitializeLayer(TLayerInstanceId instanceId, TInitData initData) {
            throw new System.NotImplementedException();
        }

        public long Tick() {
            throw new System.NotImplementedException();
        }
    }
}