// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System.Collections.Generic;
using System.Windows;
using ElephantLayer.Agents;
using ElephantLayer.TransportTypes;
using LCConnector.TransportTypes;
using LifeAPI.Layer;
using Mono.Addins;
using PlantLayer;
using TwoDimEnvironment;
using WaterLayer;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace ElephantLayer {
    [Extension(typeof (ISteppedLayer))]
    public class ElephantLayerImpl : ISteppedLayer {
        private const int _ELEPHANT_COUNT = 10;

        private readonly PlantLayerImpl _plantLayer;
        private readonly WaterLayerImpl _waterLayer;
        private readonly ITwoDimEnvironment<Elephant> _environment;
        private long _currentTick;

        public ElephantLayerImpl(PlantLayerImpl plantLayer, WaterLayerImpl waterLayer) {
            _plantLayer = plantLayer;
            _waterLayer = waterLayer;
            _environment = new TwoDimEnvironmentUseCase<Elephant>();
        }

        #region ISteppedLayer Members

        public bool InitLayer
            (TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            for (int i = 0; i < _ELEPHANT_COUNT; i++) {
                Elephant e = new Elephant
                    (i*10,
                        0,
                        new Size() {Width = 1.0, Height = 1.0},
                        -90.0,
                        60,
                        0.5,
                        _plantLayer,
                        _waterLayer,
                        _environment,
                        i);
                registerAgentHandle.Invoke(this, e);
                _environment.Add(e);
            }

            return true;
        }

        public long GetCurrentTick() {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick) {
            _currentTick = currentTick;
        }

        #endregion

        public List<TElephant> GetAllElephants() {
            List<Elephant> allElephants = _environment.GetAll();
            List<TElephant> result = new List<TElephant>();
            allElephants.ForEach(e => result.Add(new TElephant(e)));
            return result;
        }
    }
}