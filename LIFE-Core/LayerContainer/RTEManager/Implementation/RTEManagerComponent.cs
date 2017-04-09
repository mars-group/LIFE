//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System.Collections.Generic;
using LIFE.API.Agent;
using LIFE.API.Layer;
using LIFE.API.Layer.Initialization;
using NodeRegistry.Interface;
using RTEManager.Interfaces;


namespace RTEManager.Implementation
{
    public class RTEManagerComponent : IRTEManager
    {
        private readonly IRTEManager _rteManagerUseCase;

        public RTEManagerComponent(INodeRegistry nodeRegistry)
        {
            _rteManagerUseCase = new RTEManagerUseCase(nodeRegistry);
        }

        #region IRTEManager Members

        public void InitializeResultAdapter(string resultConfig)
        {
            _rteManagerUseCase.InitializeResultAdapter(resultConfig);
        }

        public void RegisterLayer(TLayerInstanceId instanceId, ILayer layer)
        {
            _rteManagerUseCase.RegisterLayer(instanceId, layer);
        }

        public void UnregisterLayer(TLayerInstanceId layerInstanceId)
        {
            _rteManagerUseCase.UnregisterLayer(layerInstanceId);
        }

        public void UnregisterTickClient(ILayer layer, ITickClient tickClient, int executionInterval = 1)
        {
            _rteManagerUseCase.UnregisterTickClient(layer, tickClient);
        }

        public void RegisterTickClient(ILayer layer, ITickClient tickClient, int executionInterval = 1)
        {
            _rteManagerUseCase.RegisterTickClient(layer, tickClient);
        }

        public bool InitializeLayer(TLayerInstanceId instanceId, TInitData initData)
        {
            return _rteManagerUseCase.InitializeLayer(instanceId, initData);
        }

        public void DisposeSuitableLayers()
        {
            _rteManagerUseCase.DisposeSuitableLayers();
        }

        public IEnumerable<ITickClient> GetAllTickClientsByLayer(TLayerInstanceId layer)
        {
            return _rteManagerUseCase.GetAllTickClientsByLayer(layer);
        }

        public long Advance(long ticksToAdvanceBy)
        {
            return _rteManagerUseCase.Advance(ticksToAdvanceBy);
        }

        #endregion
    }
}