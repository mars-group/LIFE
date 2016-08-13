//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Text;
using ProtoBuf;

namespace CommonTypes.DataTypes {
    /// <summary>
    ///     The endpoint of a node participating in the LIFE system
    /// </summary>
    [ProtoContract]
    [Serializable]
    public class NodeEndpoint {
        [ProtoMember(1)]
        public string IpAddress { get; private set; }

        [ProtoMember(2)]
        public int Port { get; private set; }

        private NodeEndpoint() {}

        public NodeEndpoint(string ipAddress, int port) {
            IpAddress = ipAddress;
            Port = port;
        }

        public override string ToString() {
            return new StringBuilder().AppendFormat("{0} IP {1} Port {2}", GetType().Name, IpAddress, Port).ToString();
        }
    }
}