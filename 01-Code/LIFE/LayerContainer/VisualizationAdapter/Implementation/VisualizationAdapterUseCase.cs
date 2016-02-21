//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using LifeAPI.Layer.Visualization;
using LIFEViewProtocol.Basics;
using VisualizationAdapter.Interface;

namespace VisualizationAdapter.Implementation {
    internal class VisualizationAdapterUseCase : IVisualizationAdapterInternal {
        private readonly ConcurrentDictionary<IVisualizableLayer, byte> _visualizables;
        private bool _isRunning;
        private int? _tickVisualizationIntervall;

        public VisualizationAdapterUseCase() {
            _visualizables = new ConcurrentDictionary<IVisualizableLayer, byte>();
            _isRunning = false;
        }

        #region IVisualizationAdapterInternal Members

        public void VisualizeTick(int currentTick) {
            if (!_isRunning) return;
            if (_tickVisualizationIntervall.HasValue && currentTick%_tickVisualizationIntervall != 0) return;


            Parallel.ForEach
                (_visualizables.Keys,
                    vis => {
                        // Get data from Layers
                        List<string> visMessages = vis.GetVisData();

                        // Send via Queue if possible
                        // TODO : Send to Queue
                    });
        }

        public void RegisterVisualizable(IVisualizableLayer visualizableLayer) {
            _visualizables.TryAdd(visualizableLayer, new byte());
        }
        public void DeRegisterVisualizable(IVisualizableLayer visTickClient)
        {
            byte bla;
            _visualizables.TryRemove(visTickClient, out bla);
        }

        public void StartVisualization(int? nrOfTicksToVisualize = null) {
            _tickVisualizationIntervall = nrOfTicksToVisualize;
            _isRunning = true;
        }

        public void StopVisualization() {
            _isRunning = false;
        }

        public void ChangeVisualizationView(double topLeft, double topRight, double bottomLeft, double bottomRight) {
            throw new NotImplementedException();
        }

        #endregion
    }
}