using System.IO;
using System.Net;
using CommonTypes.DataTypes;
using ProtoBuf;

namespace NodeRegistry.Implementation.Messages.Factory {
    internal static class NodeRegistryMessageFactory {
        public static byte[] GetJoinMessage(TNodeInformation information, string localAddress) {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, new NodeRegistryConnectionInfoMessage(NodeRegistryMessageType.Join, information, localAddress));

            return stream.ToArray();
        }

        public static byte[] GetLeaveMessage(TNodeInformation information, string localAddress)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, new NodeRegistryConnectionInfoMessage(NodeRegistryMessageType.Leave, information, localAddress));

            return stream.ToArray();
        }

        public static byte[] GetAnswerMessage(TNodeInformation information, string localAddress)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, new NodeRegistryConnectionInfoMessage(NodeRegistryMessageType.Answer, information, localAddress));

            return stream.ToArray();
        }

        public static byte[] GetHeartBeatMessage(TNodeInformation information) {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, new NodeRegistryHeartBeatMessage(NodeRegistryMessageType.HeartBeat, information.NodeIdentifier, information.NodeType));

            return stream.ToArray();
        }

    }
}