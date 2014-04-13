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
        private readonly Action<long> _tickFinishedCallback;

        public LayerContainerClient(IScsServiceClient<ILayerContainer> layerContainer,
            ModelContent content,
            IList<TLayerDescription> instantiationOrder,
            int nr,
            Action<long> tickFinishedCallback)
        {
            _layerContainer = layerContainer;
            _tickFinishedCallback = tickFinishedCallback;
            _layerContainer.Connect();

            _layerContainer.ServiceProxy.LoadModelContent(content);

            foreach (var layerDescription in instantiationOrder)
            {
                var tmp = new TLayerInstanceId(layerDescription, nr);
                _layerContainer.ServiceProxy.Instantiate(tmp);
                _layerContainer.ServiceProxy.InitializeLayer(tmp, new TInitData());
            }
        }

        public void Tick()
        {
            long tickDuration = _layerContainer.ServiceProxy.Tick();
            _tickFinishedCallback(tickDuration);
        }
    }
}
