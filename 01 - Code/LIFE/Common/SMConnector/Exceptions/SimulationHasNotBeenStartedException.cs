﻿// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System;

namespace SMConnector.Exceptions {
    public class SimulationHasNotBeenStartedException : Exception {
        public SimulationHasNotBeenStartedException(string msg)
            : base(msg) {}
    }
}