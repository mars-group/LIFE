//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
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

        public void RegisterVisualizable(IVisualizableLayer visualizableLayer)
        {
           
        }

        public void VisualizeTick(int currentTick)
        {
           
        }

        public void DeRegisterVisualizable(IVisualizableLayer visTickClient)
        {
            
        }
    }
}
