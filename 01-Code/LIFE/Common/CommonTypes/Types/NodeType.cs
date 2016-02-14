//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using ProtoBuf;

namespace CommonTypes.Types {
    [ProtoContract]
    [Serializable]
    public enum NodeType {
        LayerContainer = 0,
        SimulationManager = 1,
        SimulationController = 2,
    }
}