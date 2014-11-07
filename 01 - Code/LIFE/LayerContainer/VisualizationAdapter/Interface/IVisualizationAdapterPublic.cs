using System;
using System.Collections.Generic;
using GeoAPI.Geometries;
using LayerAPI.Interfaces;
using NetTopologySuite.Geometries;

namespace VisualizationAdapter.Interface
{

    public interface IVisualizationAdapterPublic {
        event EventHandler<List<BasicVisualizationMessage>> VisualizationUpdated;
        
        void StartVisualization();

        void StopVisualization();
        
        void ChangeVisualizationView(double topLeft, double topRight, double bottomLeft, double bottomRight);
    }
}
