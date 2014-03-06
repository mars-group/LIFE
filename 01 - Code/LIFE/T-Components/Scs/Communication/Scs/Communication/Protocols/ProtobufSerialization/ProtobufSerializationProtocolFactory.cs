using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hik.Communication.Scs.Communication.Protocols.BinarySerialization;

namespace Hik.Communication.Scs.Communication.Protocols.ProtobufSerialization
{
    class ProtobufSerializationProtocolFactory : IScsWireProtocolFactory
    {
        public IScsWireProtocol CreateWireProtocol()
        {
            return new ProtobufSerializationProtocol();
        }
    }
}
