using System;
using LNSConnector.Interface;
using LNSConnector.TransportTypes;

namespace LayerNameService.Implementation
{
    public class LayerNameServiceComponent : ILayerNameService
    {
        private readonly ILayerNameService _layerNameServiceUseCase;

        public LayerNameServiceComponent()
        {
            _layerNameServiceUseCase = new LayerNameServiceUseCase();
        }

        public TLayerNameServiceEntry ResolveLayer(Type layerType)
        {
            return _layerNameServiceUseCase.ResolveLayer(layerType);
        }

        public void RegisterLayer(Type layerType, TLayerNameServiceEntry layerNameServiceEntry)
        {
            _layerNameServiceUseCase.RegisterLayer(layerType, layerNameServiceEntry);
        }
    }
}
