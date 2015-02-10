// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using LifeAPI.Layer.Visualization;

namespace VisualizationAdapter.Interface {

    /// <summary>
    /// The internal interface for the visualization adapter. Its method's won't be avaibale via the ILayercontainer interface.
    /// </summary>
    public interface IVisualizationAdapterInternal : IVisualizationAdapterPublic {

        /// <summary>
        /// Registers a new visualizable  compoenent
        /// </summary>
        /// <param name="visualizable">The IVisualizable object to be visualized.</param>
        void RegisterVisualizable(IVisualizable visualizable);

        /// <summary>
        /// Visualizes one tick.
        /// </summary>
        /// <param name="currentTick">The current tick. Needed for sanity check.</param>
        void VisualizeTick(int currentTick);
    }
}