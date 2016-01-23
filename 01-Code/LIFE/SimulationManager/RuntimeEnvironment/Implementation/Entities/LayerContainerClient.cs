using System;
using Hik.Communication.ScsServices.Client;
using LCConnector;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using RuntimeEnvironment.Interfaces.Exceptions;

namespace RuntimeEnvironment.Implementation.Entities {
    internal class LayerContainerClient {
        private readonly IScsServiceClient<ILayerContainer> _layerContainer;

        public LayerContainerClient(IScsServiceClient<ILayerContainer> layerContainer,
            ModelContent content,
            int nr) {
            _layerContainer = layerContainer;
			// set timeout to infinite
			_layerContainer.Timeout = -1;
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
            if(!Proxy.InitializeLayer(layerInstanceId, initData))
            {
                throw new LayerFailedToInitializeException("Layer " 
                    + layerInstanceId.LayerDescription.Name 
                    + " failed to initialize. Please review your InitLayer Code! MARS LIFE will shut down now."
                    );
            }
        }
    }

}