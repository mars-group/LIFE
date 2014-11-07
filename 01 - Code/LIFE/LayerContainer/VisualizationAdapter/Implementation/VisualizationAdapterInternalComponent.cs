using System;
using System.Collections.Generic;
using LayerAPI.Interfaces;
using VisualizationAdapter.Interface;

namespace VisualizationAdapter.Implementation
{
    class VisualizationAdapterInternalComponent : IVisualizationAdapterInternal
    {
        private readonly IVisualizationAdapterInternal _visualizationAdapterInternalUseCase;

        public VisualizationAdapterInternalComponent() {
            _visualizationAdapterInternalUseCase = new VisualizationAdapterInternalUseCase();
        }


        public event EventHandler<List<BasicVisualizationMessage>> VisualizationUpdated;

        public void StartVisualization() {
            _visualizationAdapterInternalUseCase.StartVisualization();
        }

        public void StopVisualization() {
            _visualizationAdapterInternalUseCase.StopVisualization();
        }

        public void ChangeVisualizationView() {
            _visualizationAdapterInternalUseCase.ChangeVisualizationView();
        }

        public void RegisterVisualizable(IVisualizable visualizable) {
            _visualizationAdapterInternalUseCase.RegisterVisualizable(visualizable);
        }

        public void VisualizeTick() {
            _visualizationAdapterInternalUseCase.VisualizeTick();
        }
    }
}
