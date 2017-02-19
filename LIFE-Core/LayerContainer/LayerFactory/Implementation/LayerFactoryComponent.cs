//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using LayerFactory.Interface;
using LayerRegistry.Interfaces;
using LCConnector.TransportTypes.ModelStructure;
using LIFE.API.Layer;

namespace LayerFactory.Implementation {
    public class LayerFactoryComponent : ILayerFactory {
        private readonly ILayerFactory _layerFactoryUseCase;

        public LayerFactoryComponent(ILayerRegistry layerRegistry) {
            _layerFactoryUseCase = new LayerFactoryUseCase(layerRegistry);
        }

        #region ILayerFactory Members

        public ILayer GetLayer(string layerName) {
            return _layerFactoryUseCase.GetLayer(layerName);
        }

        public void LoadModelContent(ModelContent content) {
            _layerFactoryUseCase.LoadModelContent(content);
        }

        #endregion
    }
}