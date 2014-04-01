﻿using CommonTypes.TransportTypes.SimulationControl;
using LayerAPI.Interfaces;
using NodeRegistry.Interface;

namespace LayerRegistry.Implementation
{
    using System;
    using Interfaces;

    public class LayerRegistryComponent : ILayerRegistry
    {
        private readonly ILayerRegistry _layerRegistryUseCase;

        public LayerRegistryComponent(INodeRegistry nodeRegistry)
        {
            _layerRegistryUseCase = new LayerRegistryUseCase(nodeRegistry);

        }

        public ILayer RemoveLayerInstance(Type layerType)
        {
            return _layerRegistryUseCase.RemoveLayerInstance(layerType);
        }

        public void ResetLayerRegistry()
        {
            _layerRegistryUseCase.ResetLayerRegistry();
        }

        public ILayer GetRemoteLayerInstance(Type layerType)
        {
            return _layerRegistryUseCase.GetRemoteLayerInstance(layerType);
        }

        public void RegisterLayer(ILayer layer)
        {
            _layerRegistryUseCase.RegisterLayer(layer);
        }
    }
}
