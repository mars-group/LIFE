//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using LayerRegistry.Interfaces;
using LifeAPI.Layer;
using NodeRegistry.Interface;

namespace LayerRegistry.Implementation {
    public class LayerRegistryComponent : ILayerRegistry {
        private readonly ILayerRegistry _layerRegistryUseCase;

        public LayerRegistryComponent(INodeRegistry nodeRegistry, NodeRegistryConfig nodeRegistryConfig)
        {
            _layerRegistryUseCase = new LayerRegistryUseCase(nodeRegistry, nodeRegistryConfig);
        }

        #region ILayerRegistry Members

        public void RemoveLayerInstance(Type layerType) {
            _layerRegistryUseCase.RemoveLayerInstance(layerType);
        }

        public void ResetLayerRegistry() {
            _layerRegistryUseCase.ResetLayerRegistry();
        }

        public object GetLayerInstance(Type layerType) {
            return _layerRegistryUseCase.GetLayerInstance(layerType);
        }

        public void RegisterLayer(ILayer layer) {
            _layerRegistryUseCase.RegisterLayer(layer);
        }

        #endregion
    }
}