//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LCConnector.TransportTypes;
using LifeAPI.Agent;
using LifeAPI.Layer;
using NodeRegistry.Interface;
using RTEManager.Interfaces;
using VisualizationAdapter.Interface;




namespace RTEManager.Implementation {
    public class RTEManagerComponent : IRTEManager {
        private readonly IRTEManager _rteManagerUseCase;

        public RTEManagerComponent(IVisualizationAdapterInternal visualizationAdapter, INodeRegistry nodeRegistry) {
            _rteManagerUseCase = new RTEManagerUseCase(visualizationAdapter, nodeRegistry);
        }

        #region IRTEManager Members
       

        public void RegisterLayer(TLayerInstanceId instanceId, ILayer layer) {
            _rteManagerUseCase.RegisterLayer(instanceId, layer);
        }

        public void UnregisterLayer(TLayerInstanceId layerInstanceId) {
            _rteManagerUseCase.UnregisterLayer(layerInstanceId);
        }

        public void UnregisterTickClient(ILayer layer, ITickClient tickClient) {
            _rteManagerUseCase.UnregisterTickClient(layer, tickClient);
        }

        public void RegisterTickClient(ILayer layer, ITickClient tickClient) {
            _rteManagerUseCase.RegisterTickClient(layer, tickClient);
        }

        public bool InitializeLayer(TLayerInstanceId instanceId, TInitData initData) {
            return _rteManagerUseCase.InitializeLayer(instanceId, initData);
        }


        public IEnumerable<ITickClient> GetAllTickClientsByLayer(TLayerInstanceId layer) {
            return _rteManagerUseCase.GetAllTickClientsByLayer(layer);
        }

        public long AdvanceOneTick() {
            return _rteManagerUseCase.AdvanceOneTick();
        }

        #endregion
    }
}