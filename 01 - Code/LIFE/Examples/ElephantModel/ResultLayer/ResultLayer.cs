// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using ElephantLayer;
using LayerAPI.Interfaces;
using Mono.Addins;
using PlantLayer;
using WaterLayer;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace ResultLayer {
    [Extension(typeof (ISteppedLayer))]
    public class ResultLayer : ISteppedLayer {
        private readonly ElephantLayerImpl _elephantLayer;
        private readonly PlantLayerImpl _plantLayer;
        private readonly WaterLayerImpl _waterLayer;
        private long _currentTick;

        public ResultLayer(ElephantLayerImpl elephantLayer, PlantLayerImpl plantLayer, WaterLayerImpl waterlayer) {
            _elephantLayer = elephantLayer;
            _plantLayer = plantLayer;
            _waterLayer = waterlayer;
        }

        #region ISteppedLayer Members

        public bool InitLayer<I>
            (I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            MatrixToFileResultAgent m = new MatrixToFileResultAgent(_elephantLayer, _plantLayer, _waterLayer);
            registerAgentHandle.Invoke(this, m);
            return true;
        }

        public long GetCurrentTick() {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick) {
            _currentTick = currentTick;
        }

        #endregion
    }
}