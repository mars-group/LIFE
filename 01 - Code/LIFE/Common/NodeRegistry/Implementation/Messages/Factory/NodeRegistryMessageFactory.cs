using System.IO;
using CommonTypes.DataTypes;
using ProtoBuf;

namespace NodeRegistry.Implementation.Messages.Factory {
    internal static class NodeRegistryMessageFactory {
        public static byte[] GetJoinMessage(NodeInformationType informationType) {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, new NodeRegistryConnectionInfoMessage(NodeRegistryMessageType.Join, informationType));

            return stream.ToArray();
        }

        public static byte[] GetLeaveMessage(NodeInformationType informationType) {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, new NodeRegistryConnectionInfoMessage(NodeRegistryMessageType.Leave, informationType));

            return stream.ToArray();
        }

        public static byte[] GetAnswerMessage(NodeInformationType informationType) {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, new NodeRegistryConnectionInfoMessage(NodeRegistryMessageType.Answer, informationType));

            return stream.ToArray();
        }

        public static byte[] GetHeartBeatMessage(NodeInformationType informationType) {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, new NodeRegistryHeartBeatMessage(NodeRegistryMessageType.HeartBeat, informationType.NodeIdentifier, informationType.NodeType));

            return stream.ToArray();
        }

    }
}