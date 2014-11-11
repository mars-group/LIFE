using System;
using System.Collections.Generic;
using LayerAPI.Interfaces;
using LayerAPI.Interfaces.Visualization;
using MessageWrappers;
using NetTopologySuite.Geometries;
using VisualizationAdapter.Interface;

namespace VisualizationAdapter.Implementation
{
    public class VisualizationAdapterComponent : IVisualizationAdapterInternal
    {
        private readonly IVisualizationAdapterInternal _visualizationAdapterInternalUseCase;

        public VisualizationAdapterComponent() {
            _visualizationAdapterInternalUseCase = new VisualizationAdapterUseCase();
        }


        public event EventHandler<List<BasicVisualizationMessage>> VisualizationUpdated;

        public void StartVisualization() {
            _visualizationAdapterInternalUseCase.StartVisualization();
        }

        public void StopVisualization() {
            _visualizationAdapterInternalUseCase.StopVisualization();
        }

        public void ChangeVisualizationView(double topLeft, double topRight, double bottomLeft, double bottomRight) {
            _visualizationAdapterInternalUseCase.ChangeVisualizationView(topLeft, topRight, bottomLeft, bottomRight);
        }

        public void RegisterVisualizable(IVisualizable visualizable) {
            _visualizationAdapterInternalUseCase.RegisterVisualizable(visualizable);
        }

        public void VisualizeTick() {
            _visualizationAdapterInternalUseCase.VisualizeTick();
        }
    }
}
