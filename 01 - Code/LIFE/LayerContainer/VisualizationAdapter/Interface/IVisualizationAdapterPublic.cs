using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using LayerAPI.Interfaces;

namespace VisualizationAdapter.Interface
{

    public interface IVisualizationAdapterPublic {
        event EventHandler<List<BasicVisualizationMessage>> VisualizationUpdated;
        
        void StartVisualization();

        void StopVisualization();

        void ChangeVisualizationView();
    }
}
