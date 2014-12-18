using System;
using System.Collections.Generic;
using LNSConnector.Interface;
using LNSConnector.TransportTypes;

namespace LayerNameService.Implementation
{
    internal class LayerNameServiceUseCase : ILayerNameService
    {
        private readonly IDictionary<Type, TLayerNameServiceEntry> _layerMap;

        public LayerNameServiceUseCase()
        {
            _layerMap = new Dictionary<Type, TLayerNameServiceEntry>();
        }

        public TLayerNameServiceEntry ResolveLayer(Type layerType)
        {
            return _layerMap[layerType];
        }

        public void RegisterLayer(Type layerType, TLayerNameServiceEntry layerNameServiceEntry)
        {
            _layerMap.Add(layerType, layerNameServiceEntry);
        }
    }
}
