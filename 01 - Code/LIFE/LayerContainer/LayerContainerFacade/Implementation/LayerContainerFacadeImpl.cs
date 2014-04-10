using Hik.Communication.ScsServices.Service;
using LayerContainerFacade.Interfaces;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;

namespace LayerContainerFacade.Implementation {


    internal class LayerContainerFacadeImpl : ScsService, ILayerContainerFacade {
        private readonly IPartitionManager _partitionManager;
        private readonly IRTEManager _rteManager;

        public LayerContainerFacadeImpl(IPartitionManager partitionManager, IRTEManager rteManager) {
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
            _rteManager.InitializeLayer(instanceId, initData);
        }

        public long Tick() {
            return _rteManager.AdvanceOneTick();
        }
    }
}