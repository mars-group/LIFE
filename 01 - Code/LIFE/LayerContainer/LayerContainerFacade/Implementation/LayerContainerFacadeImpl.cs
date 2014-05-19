using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Service;
using LayerContainerFacade.Interfaces;
using LCConnector;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;
using LayerContainerShared;



namespace LayerContainerFacade.Implementation {


    internal class LayerContainerFacadeImpl : ScsService, ILayerContainerFacade {
        private readonly IPartitionManager _partitionManager;
        private readonly IRTEManager _rteManager;
        private IScsServiceApplication _server;

        public LayerContainerFacadeImpl(LayerContainerSettings settings, IPartitionManager partitionManager, IRTEManager rteManager) {
            _partitionManager = partitionManager;
            _rteManager = rteManager;

            _server = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(settings.NodeRegistryConfig.NodeEndPointPort));

            _server.AddService<ILayerContainer, LayerContainerFacadeImpl>(this);

            //Start server
            _server.Start();
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