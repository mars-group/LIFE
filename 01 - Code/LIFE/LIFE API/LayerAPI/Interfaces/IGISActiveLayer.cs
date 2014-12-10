﻿// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using LayerAPI.Interfaces.GIS;

namespace LayerAPI.Interfaces {
    /// <summary>
    ///     An active GIS layer, which allows to load and access GIS data
    ///     as well as hold agents.
    ///     This layer gets ticked via PreTick, Tick and PostTick.
    /// </summary>
    public interface IGISActiveLayer : ISteppedActiveLayer, IGISAccess {}
}