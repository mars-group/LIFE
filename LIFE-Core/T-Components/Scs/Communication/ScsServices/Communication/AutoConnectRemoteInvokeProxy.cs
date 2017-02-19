//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Reflection;
using Hik.Communication.Scs.Client;
using Hik.Communication.Scs.Communication;
using Hik.Communication.Scs.Communication.Messengers;

namespace Hik.Communication.ScsServices.Communication
{

#if HAS_REAL_PROXY
    using System.Runtime.Remoting.Messaging;
        /// <summary>
        ///     This class extends RemoteInvokeProxy to provide auto connect/disconnect mechanism
        ///     if client is not connected to the server when a service method is called.
        /// </summary>
        /// <typeparam name="TProxy">Type of the proxy class/interface</typeparam>
        /// <typeparam name="TMessenger">Type of the messenger object that is used to send/receive messages</typeparam>
        internal class AutoConnectRemoteInvokeProxy<TProxy, TMessenger> : RemoteInvokeProxy<TProxy, TMessenger>
            where TMessenger : IMessenger {
            /// <summary>
            ///     Reference to the client object that is used to connect/disconnect.
            /// </summary>
            private readonly IConnectableClient _client;

            /// <summary>
            ///     Creates a new AutoConnectRemoteInvokeProxy object.
            /// </summary>
            /// <param name="clientMessenger">Messenger object that is used to send/receive messages</param>
            /// <param name="client">Reference to the client object that is used to connect/disconnect</param>
            /// <param name="serviceID"></param>
            public AutoConnectRemoteInvokeProxy(RequestReplyMessenger<TMessenger> clientMessenger, IConnectableClient client,
                Guid serviceID)
                : base(clientMessenger, serviceID) {
                _client = client;
            }

            public AutoConnectRemoteInvokeProxy(RequestReplyMessenger<TMessenger> clientMessenger, IConnectableClient client)
                : base(clientMessenger) {
                _client = client;
            }

            /// <summary>
            ///     Overrides message calls and translates them to messages to remote application.
            /// </summary>
            /// <param name="msg">Method invoke message (from RealProxy base class)</param>
            /// <returns>Method invoke return message (to RealProxy base class)</returns>
            public override IMessage Invoke(IMessage msg) {
                if (_client.CommunicationState == CommunicationStates.Connected) {
                    //If already connected, behave as base class (RemoteInvokeProxy).
                    return base.Invoke(msg);
                }

                //Connect, call method and finally disconnect
                _client.Connect();
                try {
                    return base.Invoke(msg);
                }
                finally {
                    _client.Disconnect();
                }
            }
        }

#else
    /// <summary>
    ///     This class extends RemoteInvokeProxy to provide auto connect/disconnect mechanism
    ///     if client is not connected to the server when a service method is called.
    /// </summary>
    /// <typeparam name="TProxy">Type of the proxy class/interface</typeparam>
    /// <typeparam name="TMessenger">Type of the messenger object that is used to send/receive messages</typeparam>
    public class AutoConnectRemoteInvokeProxy<TProxy, TMessenger> : RemoteInvokeProxy<TProxy, TMessenger>
        where TMessenger : IMessenger
    {
        /// <summary>
        ///     Reference to the client object that is used to connect/disconnect.
        /// </summary>
        private IConnectableClient _client;


        public void Configure(RequestReplyMessenger<TMessenger> clientMessenger, IConnectableClient client,
            Guid serviceID)
        {
            _client = client;
            Configure(clientMessenger, serviceID);
        }

        public void Configure(RequestReplyMessenger<TMessenger> clientMessenger, IConnectableClient client)
        {
            _client = client;
            Configure(clientMessenger);
        }

        /// <summary>
        ///     Overrides message calls and translates them to messages to remote application.
        /// </summary>
        /// <param name="msg">Method invoke message (from RealProxy base class)</param>
        /// <returns>Method invoke return message (to RealProxy base class)</returns>
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (!_configured) { throw new Exception("Proxy not configured"); }

            if (_client.CommunicationState == CommunicationStates.Connected)
            {
                //If already connected, behave as base class (RemoteInvokeProxy).
                return base.Invoke(targetMethod, args);
            }

            //Connect, call method and finally disconnect
            _client.Connect();
            try
            {
                return base.Invoke(targetMethod, args);
            }
            finally
            {
                _client.Disconnect();
            }
        }
    }
#endif
}
