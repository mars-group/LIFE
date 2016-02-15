//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
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