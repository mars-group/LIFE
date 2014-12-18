using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LNSConnector.Interface;
using LNSConnector.TransportTypes;

namespace LayerNameService.Implementation
{
    internal class LayerNameServiceUseCase : ILayerNameService
    {
        private readonly IDictionary<Type, List<TLayerNameServiceEntry>> _layerMap;

        public LayerNameServiceUseCase()
        {
            _layerMap = new ConcurrentDictionary<Type, List<TLayerNameServiceEntry>>();
        }

        public TLayerNameServiceEntry ResolveLayer(Type layerType)
        {
            return _layerMap[layerType].FirstOrDefault();
        }

        public void RegisterLayer(Type layerType, TLayerNameServiceEntry layerNameServiceEntry)
        {
            if (!_layerMap.ContainsKey(layerType))
            {
                _layerMap[layerType] = new List<TLayerNameServiceEntry>();
            }
            _layerMap[layerType].Add(layerNameServiceEntry);
        }

        public void RemoveLayer(Type layerType, TLayerNameServiceEntry layerNameServiceEntry)
        {
            if (_layerMap[layerType].Count <= 1)
            {
                _layerMap.Remove(layerType);
            }
            else
            {
                _layerMap[layerType].Remove(layerNameServiceEntry);
            }

        }
    }
}
