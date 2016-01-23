namespace ASC.Communication.Scs.Communication.Protocols.ProtobufSerialization {
    internal class ProtobufSerializationProtocolFactory : IAcsWireProtocolFactory {
        public IAcsWireProtocol CreateWireProtocol() {
            return new ProtobufSerializationProtocol();
        }
    }
}