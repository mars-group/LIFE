//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System.Collections.Generic;
using GeoAPI.Geometries;
using LIFEViewProtocol.Basics;
using LIFEViewProtocol.Helper;

namespace LifeAPI.Layer.Visualization {
    /// <summary>
    ///     Implement this interface if you want your Layer to be visualized after each tick
    /// </summary>
    public interface IVisualizable {

        /// <summary>
        ///     Returns every entity in the whole environment
        /// </summary>
        /// <returns>A list of everything visualizable in the whole environment, empty list if nothing is present</returns>
        List<BasicVisualizationMessage> GetVisData();

        /// <summary>
        ///     Returns every entity which is associated with the intersection of the provided geometry
        ///     and the environment
        /// </summary>
        /// <param name="geometry">The geometry to intersect with.</param>
        /// <returns>A list of everything visualizable in the whole environment, empty list if nothing is present</returns>
        List<BasicVisualizationMessage> GetVisData(IGeometry geometry);
    }
}