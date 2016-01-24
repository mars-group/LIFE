using System;
using System.IO;
using MsgPack;
using MsgPack.Serialization;
using NUnit.Framework;

namespace AgentShadowingServiceTests
{
    [TestFixture]
    public class MsgPackTest
    {
        [Test]
        public void TestMsgPackSerialization()
        {
            var msg = new MockObject
            {
                Parameters = new object[] {0, new MockMockObject(), "hallo", 0.5342, Guid.NewGuid()}
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

        public void PackToMessage(Packer packer, PackingOptions options)
        {
            // ...Instead, you can pack as map as follows:
            packer.PackMapHeader(Parameters.Length);
            foreach (var parameter in Parameters)
            {
                packer.Pack(parameter.GetType().ToString());
                packer.Pack(parameter);
            }
        }

        public void UnpackFromMessage(Unpacker unpacker)
        {
            // Unpack fields are here:
            // temp variables
            this.Parameters = new object[unpacker.ItemsCount];


            // ...Instead, you can unpack from map as follows:
            if (!unpacker.IsMapHeader)
            {
                throw SerializationExceptions.NewIsNotMapHeader();
            }


            // Unpack fields here:
            for (var i = 0; i < unpacker.ItemsCount; i++)
            {
                // Unpack and verify key of entry in map.
                string key;
                if (!unpacker.ReadString(out key))
                {
                    // Missing key, incorrect.
                    throw SerializationExceptions.NewUnexpectedEndOfStream();
                }

                var currentType = Type.GetType(key);
                var item = unpacker.ReadItem();
                
                Console.WriteLine(item.Value.GetType());

            }
        }
    }

    [Serializable]
    public class MockMockObject
    {
        public int FourtyTwo { get; set; }

        public MockMockObject()
        {
            FourtyTwo = 42;
        }
    }
}