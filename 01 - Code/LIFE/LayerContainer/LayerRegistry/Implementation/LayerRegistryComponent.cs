// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 13.05.2014
//  *******************************************************/

using System;
using DistributedKeyValueStore.Interface;
using LayerAPI.Layer;
using LayerRegistry.Interfaces;
using LayerRegistry.Interfaces.Config;

namespace LayerRegistry.Implementation {
    public class LayerRegistryComponent : ILayerRegistry {
        private readonly ILayerRegistry _layerRegistryUseCase;

        public LayerRegistryComponent
            (IDistributedKeyValueStore distributedKeyValueStore, LayerRegistryConfig layerRegistryConfig) {
            _layerRegistryUseCase = new LayerRegistryUseCase(distributedKeyValueStore, layerRegistryConfig);
        }

        #region ILayerRegistry Members

        public ILayer RemoveLayerInstance(Type layerType) {
            return _layerRegistryUseCase.RemoveLayerInstance(layerType);
        }

        public void ResetLayerRegistry() {
            _layerRegistryUseCase.ResetLayerRegistry();
        }

        public ILayer GetLayerInstance(Type layerType) {
            return _layerRegistryUseCase.GetLayerInstance(layerType);
        }

        public void RegisterLayer(ILayer layer) {
            _layerRegistryUseCase.RegisterLayer(layer);
        }

        #endregion
    }
}