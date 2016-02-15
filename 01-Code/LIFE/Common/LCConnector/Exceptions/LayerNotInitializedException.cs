//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using LCConnector.TransportTypes;

namespace LCConnector.Exceptions {
    public class LayerNotInitializedException : Exception {
        public TLayerInstanceId Id { get; protected set; }

        public LayerNotInitializedException(string message, TLayerInstanceId id)
            : base(message) {
            Id = id;
        }

        public LayerNotInitializedException(TLayerInstanceId id) {
            Id = id;
        }
    }
}