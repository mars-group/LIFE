//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System.Text;
using CommonTypes.DataTypes;
using Newtonsoft.Json;

namespace NodeRegistry.Implementation.Messages.Factory
{
    internal static class NodeRegistryMessageFactory
    {
        private static readonly JsonSerializerSettings Jset = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static byte[] GetJoinMessage(TNodeInformation information, string localAddress, string clusterName)
        {
            var json =
                JsonConvert.SerializeObject(
                    new NodeRegistryConnectionInfoMessage(NodeRegistryMessageType.Join, information, localAddress,
                        clusterName),
                    Jset);
            return Encoding.UTF8.GetBytes(json);
        }

        public static byte[] GetLeaveMessage(TNodeInformation information, string localAddress, string clusterName)
        {
            var json =
                JsonConvert.SerializeObject(
                    new NodeRegistryConnectionInfoMessage(NodeRegistryMessageType.Leave, information, localAddress,
                        clusterName),
                    Jset);

            return Encoding.UTF8.GetBytes(json);
        }

        public static byte[] GetAnswerMessage(TNodeInformation information, string localAddress, string clusterName)
        {
            var json =
                JsonConvert.SerializeObject(
                    new NodeRegistryConnectionInfoMessage(NodeRegistryMessageType.Answer, information, localAddress,
                        clusterName),
                    Jset);

            return Encoding.UTF8.GetBytes(json);
        }

        public static byte[] GetHeartBeatMessage(TNodeInformation information, string clusterName)
        {
            var json =
                JsonConvert.SerializeObject(
                    new NodeRegistryHeartBeatMessage(NodeRegistryMessageType.HeartBeat, information.NodeIdentifier,
                        information.NodeType, clusterName), Jset);

            return Encoding.UTF8.GetBytes(json);
        }
    }
}