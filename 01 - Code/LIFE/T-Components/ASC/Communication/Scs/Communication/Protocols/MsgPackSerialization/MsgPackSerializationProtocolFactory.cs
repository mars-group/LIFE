using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Communication.Scs.Communication.Protocols.MsgPackSerialization
{
    class MsgPackSerializationProtocolFactory : IAcsWireProtocolFactory
    {
        public IAcsWireProtocol CreateWireProtocol() {
            return new MsgPackSerializationProtocol();
        }
    }
}
