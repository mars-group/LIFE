// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 13.05.2014
//  *******************************************************/

using LayerFactory.Interface;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using LifeAPI.Layer;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;

namespace PartitionManager.Implementation {
    internal class PartitionManagerUseCase : IPartitionManager {
        private readonly ILayerFactory _layerFactory;

        private readonly IRTEManager _rteManager;

        public PartitionManagerUseCase(ILayerFactory layerFactory, IRTEManager rteManager) {
            _layerFactory = layerFactory;
            _rteManager = rteManager;
        }

        #region IPartitionManager Members

        public bool AddLayer(TLayerInstanceId instanceId) {
            ILayer layer = _layerFactory.GetLayer(instanceId.LayerDescription.Name);
            _rteManager.RegisterLayer(instanceId, layer);
            return true;
        }

        public void LoadModelContent(ModelContent content) {
            _layerFactory.LoadModelContent(content);
        }

        #endregion
    }
}