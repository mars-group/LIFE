//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using LayerFactory.Interface;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;

namespace PartitionManager.Implementation {
    public class PartitionManagerComponent : IPartitionManager {
        private readonly PartitionManagerUseCase _partitionManagerUseCase;

        public PartitionManagerComponent(ILayerFactory layerFactory, IRTEManager rteManager) {
            _partitionManagerUseCase = new PartitionManagerUseCase(layerFactory, rteManager);
        }

        #region IPartitionManager Members

        public bool AddLayer(TLayerInstanceId instanceId) {
            return _partitionManagerUseCase.AddLayer(instanceId);
        }

        public void LoadModelContent(ModelContent content) {
            _partitionManagerUseCase.LoadModelContent(content);
        }

        #endregion
    }
}