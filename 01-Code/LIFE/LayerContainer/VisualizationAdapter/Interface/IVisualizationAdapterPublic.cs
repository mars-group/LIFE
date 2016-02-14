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
using LIFEViewProtocol.Basics;

namespace VisualizationAdapter.Interface {
    /// <summary>
    /// The public interface of the VisualizationAdapter
    /// </summary>
    public interface IVisualizationAdapterPublic {
        /// <summary>
        /// An event indicating that the Visualization has been updated.
        /// The event args will contain all changes.
        /// </summary>
        event EventHandler<List<BasicVisualizationMessage>> VisualizationUpdated;

        /// <summary>
        /// Starts the visualization. Takes optional parameter to 
        /// set the number of ticks to visualize at once.
        /// </summary>
        /// <param name="nrOfTicksToVisualize">The # of ticks to visualize.</param>
        void StartVisualization(int? nrOfTicksToVisualize = null);

        /// <summary>
        /// Stops the visualization.
        /// </summary>
        void StopVisualization();

        /// <summary>
        /// NOT IMPLEMENTED YET!
        /// Adjusts the visualization's camera view. 
        /// </summary>
        /// <param name="topLeft">The topleft corner of the view angle</param>
        /// <param name="topRight">The topright corner of the view angle</param>
        /// <param name="bottomLeft">The bottomLeft corner of the view angle</param>
        /// <param name="bottomRight">The bottomright corner of the view angle</param>
        void ChangeVisualizationView(double topLeft, double topRight, double bottomLeft, double bottomRight);
    }
}