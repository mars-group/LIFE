//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
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