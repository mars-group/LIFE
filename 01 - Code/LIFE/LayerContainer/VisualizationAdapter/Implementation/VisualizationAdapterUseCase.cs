﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LayerAPI.Interfaces.Visualization;
using MessageWrappers;
using VisualizationAdapter.Interface;

namespace VisualizationAdapter.Implementation
{
    internal class VisualizationAdapterUseCase : IVisualizationAdapterInternal {
        private readonly List<IVisualizable> _visualizables;
        private bool _isRunning;
        private int? _tickVisualizationIntervall;

        public VisualizationAdapterUseCase() {
            _visualizables = new List<IVisualizable>();
            _isRunning = false;
        }

        public event EventHandler<List<BasicVisualizationMessage>> VisualizationUpdated;

        public void VisualizeTick(int currentTick) {
            if (!_isRunning) return;
            if (_tickVisualizationIntervall.HasValue && currentTick % _tickVisualizationIntervall != 0) return;


            Parallel.ForEach(_visualizables,
                vis => {
                    // Get data from Layers
                    var visMessages = vis.GetVisData();
                    
                    // Raise Event for everbody locally interested
                    OnRaiseVisualizationUpdated(visMessages);

                    // Send via Queue if possible
                    // TODO: Send via Queue
                });
        }

        private void OnRaiseVisualizationUpdated(List<BasicVisualizationMessage> visMessages) {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var handler = VisualizationUpdated;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, visMessages);
            }
        }

        public void RegisterVisualizable(IVisualizable visualizable)
        {
            _visualizables.Add(visualizable);
        }

        public void StartVisualization(int? nrOfTicksToVisualize = null) {
            _tickVisualizationIntervall = nrOfTicksToVisualize;
            this._isRunning = true;
        }

        public void StopVisualization() {
            this._isRunning = false;
        }

        public void ChangeVisualizationView(double topLeft, double topRight, double bottomLeft, double bottomRight) {
            throw new NotImplementedException();
        }


    }
}