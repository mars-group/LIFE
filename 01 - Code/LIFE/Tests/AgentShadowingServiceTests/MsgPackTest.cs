using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ASC.Communication.Scs.Communication.Messages;
using ASC.Communication.ScsServices.Communication.Messages;
using MsgPack;
using MsgPack.Serialization;
using NUnit.Framework;

namespace AgentShadowingServiceTests
{
    [TestFixture]
    public class MsgPackTest
    {

        [Test]
        public void TestMsgPackSerialization() {

            var msg = new MockObject
            {
                Parameters = new object[] { 0, new MockMockObject(), "hallo", 0.5342, Guid.NewGuid() },
            };

            var serializer = MessagePackSerializer.Get<MockObject>();



            Stream stream = new MemoryStream();
            serializer.Pack(stream, msg);

            Console.WriteLine("Size is : " + stream.Length + " bytes");

            stream.Position = 0;


            var value = serializer.Unpack(stream);
            
            foreach (var v in value.Parameters)
            {
                Console.WriteLine(v);
            }

        }

    }

    [Serializable]
    public class MockObject
    {
        public object[] Parameters { get; set; }
    }

    [Serializable]
    public class MockMockObject
    {
        private int fourtyTwo;

        public MockMockObject()
        {
            fourtyTwo = 42;
        }
    }
}
