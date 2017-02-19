﻿//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using Hik.Communication.Scs.Communication.Protocols;

namespace Scs.Communication.Scs.Communication.Protocols.JsonSerialization {
    /// <summary>
    ///     This class is used to create Binary Serialization Protocol objects.
    /// </summary>
    public class JsonSerializationProtocolFactory : IScsWireProtocolFactory {
        /// <summary>
        ///     Creates a new Wire Protocol object.
        /// </summary>
        /// <returns>Newly created wire protocol object</returns>
        public IScsWireProtocol CreateWireProtocol() {
            return new JsonSerializationProtocol();
        }
    }
}