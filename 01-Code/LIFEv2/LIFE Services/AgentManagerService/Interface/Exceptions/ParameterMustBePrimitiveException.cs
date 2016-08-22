﻿//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 18.12.2015
//  *******************************************************/
using System;
using System.Runtime.Serialization;

namespace AgentManager.Interface.Exceptions {
    [Serializable]
    public class ParameterMustBePrimitiveException : Exception {
        public ParameterMustBePrimitiveException(string msg) : base(msg) {
            
        }

		public ParameterMustBePrimitiveException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context) { }
    }
}