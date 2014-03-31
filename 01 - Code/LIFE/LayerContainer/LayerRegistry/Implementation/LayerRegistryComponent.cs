using CommonTypes.TransportTypes.SimulationControl;
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
        public ILayer RemoveLayerInstance(LayerInstanceIdType layerInstanceId)
        {
            return _layerRegistryUseCase.RemoveLayerInstance(layerInstanceId);
        }

        public ILayer RemoveLayerInstance(Type layerType)
        {
            return _layerRegistryUseCase.RemoveLayerInstance(layerType);
        }

        public void ResetLayerRegistry()
        {
            _layerRegistryUseCase.ResetLayerRegistry();
        }

        public ILayer GetLayerInstance(Type parameterType)
        {
            return _layerRegistryUseCase.GetLayerInstance(parameterType);
        }

        public ILayer GetLayerInstance(LayerInstanceIdType layerInstanceId)
        {
            return _layerRegistryUseCase.GetLayerInstance(layerInstanceId);
        }

        public void RegisterLayer(ILayer layer)
        {
            _layerRegistryUseCase.RegisterLayer(layer);
        }
    }
}
