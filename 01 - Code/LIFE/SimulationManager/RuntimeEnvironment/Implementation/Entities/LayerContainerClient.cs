using System.Collections.Generic;
using Hik.Communication.ScsServices.Client;
using LCConnector;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;

namespace RuntimeEnvironment.Implementation.Entities {
    internal class LayerContainerClient {
        private readonly IScsServiceClient<ILayerContainer> _layerContainer;

        public LayerContainerClient(IScsServiceClient<ILayerContainer> layerContainer,
            ModelContent content,
            int nr) {
            _layerContainer = layerContainer;
            _layerContainer.Connect();

            _layerContainer.ServiceProxy.LoadModelContent(content);
        }

        public void Instantiate(TLayerInstanceId instanceId) {
            _layerContainer.ServiceProxy.Instantiate(instanceId);
        }

        public long Tick() {
            return _layerContainer.ServiceProxy.Tick();
        }

        public ILayerContainer Proxy {
            get { return _layerContainer.ServiceProxy; }
        }

        public void Initialize(TLayerInstanceId layerInstanceId, TInitData initData)
        {
            Proxy.InitializeLayer(layerInstanceId, initData);
        }
    }
}