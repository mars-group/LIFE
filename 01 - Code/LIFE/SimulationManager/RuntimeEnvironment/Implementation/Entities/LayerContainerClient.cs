using System;
using System.Collections.Generic;
using Hik.Communication.ScsServices.Client;
using LCConnector;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;

namespace RuntimeEnvironment.Implementation.Entities
{

    internal class LayerContainerClient
    {
        private readonly IScsServiceClient<ILayerContainer> _layerContainer;

        public LayerContainerClient(IScsServiceClient<ILayerContainer> layerContainer,
            ModelContent content,
            IEnumerable<TLayerDescription> instantiationOrder,
            int nr)
        {
            _layerContainer = layerContainer;
            _layerContainer.Connect();

            _layerContainer.ServiceProxy.LoadModelContent(content);

            foreach (var layerDescription in instantiationOrder)
            {
                var tmp = new TLayerInstanceId(layerDescription, nr);
                _layerContainer.ServiceProxy.Instantiate(tmp);
                _layerContainer.ServiceProxy.InitializeLayer(tmp, new TInitData());
            }
        }

        public long Tick()
        {
            return _layerContainer.ServiceProxy.Tick();
        }
    }
}
