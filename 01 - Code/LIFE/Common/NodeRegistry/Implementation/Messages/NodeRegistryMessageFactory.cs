using System.IO;
using CommonTypes.DataTypes;
using ProtoBuf;

namespace NodeRegistry.Implementation.Messages {
    internal static class NodeRegistryMessageFactory {
        public static byte[] GetJoinMessage(NodeInformationType informationType) {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, new NodeRegistryMessage(NodeRegistryMessageType.Join, informationType));

            return stream.ToArray();
        }

        public static byte[] GetLeaveMessage(NodeInformationType informationType) {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, new NodeRegistryMessage(NodeRegistryMessageType.Leave, informationType));

            return stream.ToArray();
        }

        public static byte[] GetAnswerMessage(NodeInformationType informationType) {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, new NodeRegistryMessage(NodeRegistryMessageType.Answer, informationType));

            return stream.ToArray();
        }

        public static byte[] GetHeartBeatMessage(NodeInformationType informationType) {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, new NodeRegistryMessage(NodeRegistryMessageType.HeartBeat, informationType));

            return stream.ToArray();
        }

    }
}