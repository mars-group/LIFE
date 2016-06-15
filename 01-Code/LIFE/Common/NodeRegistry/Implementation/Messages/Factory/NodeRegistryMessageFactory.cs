//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System.IO;
using CommonTypes.DataTypes;
using ProtoBuf;

namespace NodeRegistry.Implementation.Messages.Factory {
    internal static class NodeRegistryMessageFactory {
        public static byte[] GetJoinMessage(TNodeInformation information, string localAddress, string clusterName) {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, new NodeRegistryConnectionInfoMessage(NodeRegistryMessageType.Join, information, localAddress, clusterName));

            return stream.ToArray();
        }

        public static byte[] GetLeaveMessage(TNodeInformation information, string localAddress, string clusterName)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, new NodeRegistryConnectionInfoMessage(NodeRegistryMessageType.Leave, information, localAddress, clusterName));

            return stream.ToArray();
        }

        public static byte[] GetAnswerMessage(TNodeInformation information, string localAddress, string clusterName)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, new NodeRegistryConnectionInfoMessage(NodeRegistryMessageType.Answer, information, localAddress, clusterName));

            return stream.ToArray();
        }

        public static byte[] GetHeartBeatMessage(TNodeInformation information, string clusterName) {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, new NodeRegistryHeartBeatMessage(NodeRegistryMessageType.HeartBeat, information.NodeIdentifier, information.NodeType, clusterName));

            return stream.ToArray();
        }

    }
}