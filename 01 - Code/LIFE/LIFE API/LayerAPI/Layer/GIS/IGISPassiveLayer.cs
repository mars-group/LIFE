// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

namespace LayerAPI.Layer.GIS {
    /// <summary>
    ///     A passive GIS layer, which allows to load and access GIS data
    ///     as well as hold agents.
    /// </summary>
    public interface IGISPassiveLayer : ISteppedLayer, IGISAccess {}
}