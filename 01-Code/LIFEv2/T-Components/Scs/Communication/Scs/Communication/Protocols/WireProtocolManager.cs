//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using Scs.Communication.Scs.Communication.Protocols.JsonSerialization;

namespace Hik.Communication.Scs.Communication.Protocols {
    /// <summary>
    ///     This class is used to get default protocols.
    /// </summary>
    internal static class WireProtocolManager {
        /// <summary>
        ///     Creates a default wire protocol factory object to be used on communicating of applications.
        /// </summary>
        /// <returns>A new instance of default wire protocol</returns>
        public static IScsWireProtocolFactory GetDefaultWireProtocolFactory() {
            return new JsonSerializationProtocolFactory();
            //return new ProtobufSerializationProtocolFactory();
        }

        /// <summary>
        ///     Creates a default wire protocol object to be used on communicating of applications.
        /// </summary>
        /// <returns>A new instance of default wire protocol</returns>
        public static IScsWireProtocol GetDefaultWireProtocol() {
            return new JsonSerializationProtocol();
            //return new ProtobufSerializationProtocol();
        }
    }
}