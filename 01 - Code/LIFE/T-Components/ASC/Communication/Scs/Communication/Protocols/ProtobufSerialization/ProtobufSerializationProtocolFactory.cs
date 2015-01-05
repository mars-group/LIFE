namespace ASC.Communication.Scs.Communication.Protocols.ProtobufSerialization {
    internal class ProtobufSerializationProtocolFactory : IScsWireProtocolFactory {
        public IScsWireProtocol CreateWireProtocol() {
            return new ProtobufSerializationProtocol();
        }
    }
}