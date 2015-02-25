// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LifeAPI.Layer.Visualization;
using LIFEViewProtocol.Basics;
using VisualizationAdapter.Interface;

namespace VisualizationAdapter.Implementation {
    internal class VisualizationAdapterUseCase : IVisualizationAdapterInternal {
        private readonly List<IVisualizable> _visualizables;
        private bool _isRunning;
        private int? _tickVisualizationIntervall;

        public VisualizationAdapterUseCase() {
            _visualizables = new List<IVisualizable>();
            _isRunning = false;
        }

        #region IVisualizationAdapterInternal Members

        public event EventHandler<List<BasicVisualizationMessage>> VisualizationUpdated;

        public void VisualizeTick(int currentTick) {
            if (!_isRunning) return;
            if (_tickVisualizationIntervall.HasValue && currentTick%_tickVisualizationIntervall != 0) return;


            Parallel.ForEach
                (_visualizables,
                    vis => {
                        // Get data from Layers
                        List<BasicVisualizationMessage> visMessages = vis.GetVisData();

                        // Raise Event for everbody locally interested
                        OnRaiseVisualizationUpdated(visMessages);

                        // Send via Queue if possible
                        // TODO : Send to Queue
                    });
        }

        public void RegisterVisualizable(IVisualizable visualizable) {
            _visualizables.Add(visualizable);
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

        private void OnRaiseVisualizationUpdated(List<BasicVisualizationMessage> visMessages) {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<List<BasicVisualizationMessage>> handler = VisualizationUpdated;

            // Event will be null if there are no subscribers
            if (handler != null) handler(this, visMessages);
        }
    }
}