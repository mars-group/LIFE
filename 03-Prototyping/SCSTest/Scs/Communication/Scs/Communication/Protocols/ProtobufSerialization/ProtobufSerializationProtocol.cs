using System.Collections.Generic;
using System.IO;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.ScsServices.Communication.Messages;
using ProtoBuf;
using ProtoBuf.Meta;

namespace Hik.Communication.Scs.Communication.Protocols.ProtobufSerialization {
    internal class ProtobufSerializationProtocol : IScsWireProtocol {
        public ProtobufSerializationProtocol() {
            RuntimeTypeModel.Default.Add(typeof (IScsMessage), true);
            RuntimeTypeModel.Default.Add(typeof (ScsRemoteInvokeMessage), false)
                .Add("ServiceClassName", "MethodName", "Parameters");
            RuntimeTypeModel.Default.Add(typeof (ScsRemoteInvokeReturnMessage), false)
                .Add("ReturnValue", "RemoteException");
            RuntimeTypeModel.Default.Add(typeof (ScsRemoteException), true);
        }

        public byte[] GetBytes(IScsMessage message) {
            var stream = new MemoryStream();
            if (message is ScsRemoteInvokeMessage) {
                var msg = message as ScsRemoteInvokeMessage;
                var t = msg.Parameters[0].GetType();
                RuntimeTypeModel.Default.Add(t, true);
            }

            Serializer.Serialize(stream, message);
            return stream.ToArray();
        }

        public IEnumerable<IScsMessage> CreateMessages(byte[] receivedBytes) {
            var msg = Serializer.Deserialize<IScsMessage>(new MemoryStream(receivedBytes));
            var list = new List<IScsMessage>();
            if (msg != null) list.Add(msg);
            return list;
        }

        public void Reset() {}
    }
}