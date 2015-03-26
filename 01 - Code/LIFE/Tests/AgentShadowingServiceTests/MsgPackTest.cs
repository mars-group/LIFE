using System;
using System.IO;
using ASC.Communication.Scs.Communication.Messages;
using ASC.Communication.ScsServices.Communication.Messages;
using MsgPack.Serialization;
using NUnit.Framework;

namespace AgentShadowingServiceTests
{
    [TestFixture]
    public class MsgPackTest
    {

        [Test]
        public void TestMsgPackSerialization() {
            var serializer = MessagePackSerializer.Get<AscMessage>();
            var msg = new AscRemoteInvokeMessage {
                MethodName = "TestMethod",
                Parameters = new object[] { 0,"hallo",0.5342, Guid.NewGuid()},
                MessageId = Guid.NewGuid().ToString()
            };
            Stream stream = new MemoryStream();
            serializer.Pack(stream, msg);
            stream.Position = 0;

            var value = serializer.Unpack(stream);
            Console.WriteLine(value);
        }

    }
}
