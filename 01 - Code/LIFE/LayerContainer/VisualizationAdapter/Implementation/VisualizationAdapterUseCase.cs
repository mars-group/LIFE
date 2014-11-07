using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LayerAPI.Interfaces;
using VisualizationAdapter.Interface;

namespace VisualizationAdapter.Implementation
{
    internal class VisualizationAdapterUseCase : IVisualizationAdapterInternal {
        private readonly List<IVisualizable> _visualizables;
        private bool _isRunning;

        public VisualizationAdapterUseCase() {
            _visualizables = new List<IVisualizable>();
            _isRunning = false;
        }

        public event EventHandler<List<BasicVisualizationMessage>> VisualizationUpdated;

        public void VisualizeTick() {
            if (!_isRunning) return;

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
                // Use the () operator to raise the event.
                handler(this, visMessages);
            }
        }

        public void RegisterVisualizable(IVisualizable visualizable)
        {
            _visualizables.Add(visualizable);
        }

        public void StartVisualization() {
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
