using System;
using System.Collections.Generic;
using GeoAPI.Geometries;
using LayerAPI.Interfaces;
using MessageWrappers;
using NetTopologySuite.Geometries;

namespace VisualizationAdapter.Interface
{

    public interface IVisualizationAdapterPublic {
        event EventHandler<List<BasicVisualizationMessage>> VisualizationUpdated;

        void StartVisualization(int? nrOfTicksToVisualize = null);

        void StopVisualization();
        
        void ChangeVisualizationView(double topLeft, double topRight, double bottomLeft, double bottomRight);
    }
}
