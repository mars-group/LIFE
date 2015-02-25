using System;
using System.Collections.Generic;
using LifeAPI.Layer.Visualization;
using LIFEViewProtocol.Basics;
using VisualizationAdapter.Interface;

namespace RTEManagerBlackBoxTest.Mocks
{
    class VisualizationAdapterInternalMock : IVisualizationAdapterInternal
    {
       public event EventHandler<List<BasicVisualizationMessage>> VisualizationUpdated;

        public void StartVisualization(int? nrOfTicksToVisualize = null)
        {
            
        }

        public void StopVisualization()
        {
            
        }

        public void ChangeVisualizationView(double topLeft, double topRight, double bottomLeft, double bottomRight)
        {
         
        }

        public void RegisterVisualizable(IVisualizable visualizable)
        {
           
        }

        public void VisualizeTick(int currentTick)
        {
           
        }
    }
}
