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
using ASC.Communication.Scs.Communication.Messages;
using ASC.Communication.ScsServices.Communication.Messages;
using ProtoBuf;
using ProtoBuf.Meta;

namespace ASC.Communication.Scs.Communication.Protocols.ProtobufSerialization {
    internal class ProtobufSerializationProtocol : IAcsWireProtocol {
        public ProtobufSerializationProtocol() {
            RuntimeTypeModel.Default.Add(typeof (IAscMessage), true);
            RuntimeTypeModel.Default.Add(typeof (AscRemoteInvokeMessage), false)
                .Add("ServiceClassName", "MethodName", "Parameters");
            RuntimeTypeModel.Default.Add(typeof (AscRemoteInvokeReturnMessage), false)
                .Add("ReturnValue", "RemoteException");
            RuntimeTypeModel.Default.Add(typeof (AcsRemoteException), true);
        }

        public byte[] GetBytes(IAscMessage message) {
            var stream = new MemoryStream();
            if (message is AscRemoteInvokeMessage) {
                var msg = message as AscRemoteInvokeMessage;
                var t = msg.Parameters[0].GetType();
                RuntimeTypeModel.Default.Add(t, true);
            }

            Serializer.Serialize(stream, message);
            return stream.ToArray();
        }

        public IEnumerable<IAscMessage> CreateMessages(byte[] receivedBytes) {
            var msg = Serializer.Deserialize<IAscMessage>(new MemoryStream(receivedBytes));
            var list = new List<IAscMessage>();
            if (msg != null) list.Add(msg);
            return list;
        }

        public void Reset() {}
    }
}