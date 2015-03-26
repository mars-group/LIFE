using System;
using System.Collections.Generic;
using System.IO;
using ASC.Communication.Scs.Communication.Messages;
using MsgPack.Serialization;

namespace ASC.Communication.Scs.Communication.Protocols.MsgPackSerialization
{
    class MsgPackSerializationProtocol : IAcsWireProtocol
    {
        private MessagePackSerializer<IAscMessage> _serializer;

        public MsgPackSerializationProtocol() {
            _serializer = MessagePackSerializer.Get<IAscMessage>();
        }

        public byte[] GetBytes(IAscMessage message) {
            var stream = new MemoryStream();
            _serializer.Pack(stream, message);
            return stream.ToArray();
        }

        public IEnumerable<IAscMessage> CreateMessages(byte[] receivedBytes) {
            throw new NotImplementedException();
        }

        public void Reset() {
            throw new NotImplementedException();
        }
    }
}
