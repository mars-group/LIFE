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
using ASC.Communication.Scs.Communication;
using ASC.Communication.Scs.Communication.EndPoints;
using ASC.Communication.Scs.Communication.Messengers;
using ASC.Communication.Scs.Server;
using ASC.Communication.ScsServices.Communication;
using ASC.Communication.ScsServices.Service;

namespace ASC.Communication.AscServices.Service {
#if HAS_REAL_PROXY

    using System.Runtime.Remoting.Proxies;


    /// <summary>
    ///     Implements IAscServiceClient.
    ///     It is used to manage and monitor a service client.
    /// </summary>
    internal class AscServiceClient : IAscServiceClient {
    #region Public events

        /// <summary>
        ///     This event is raised when this client is disconnected from server.
        /// </summary>
        public event EventHandler Disconnected;

    #endregion

    #region Public properties

        /// <summary>
        ///     Unique identifier for this client.
        /// </summary>
        public long ClientId {
            get { return _serverClient.ClientId; }
        }

        /// <summary>
        ///     Gets endpoint of remote application.
        /// </summary>
        public AscEndPoint RemoteEndPoint {
            get { return _serverClient.RemoteEndPoint; }
        }

        /// <summary>
        ///     Gets the communication state of the Client.
        /// </summary>
        public CommunicationStates CommunicationState {
            get { return _serverClient.CommunicationState; }
        }

    #endregion

    #region Private fields

        /// <summary>
        ///     Reference to underlying IAscServerClient object.
        /// </summary>
        private readonly IAscServerClient _serverClient;

        /// <summary>
        ///     This object is used to send messages to client.
        /// </summary>
        private readonly RequestReplyMessenger<IAscServerClient> _requestReplyMessenger;

        /// <summary>
        ///     Last created proxy object to invoke remote medhods.
        /// </summary>
        private RealProxy _realProxy;

    #endregion

    #region Constructor

        /// <summary>
        ///     Creates a new AscServiceClient object.
        /// </summary>
        /// <param name="serverClient">Reference to underlying IAscServerClient object</param>
        /// <param name="requestReplyMessenger">RequestReplyMessenger to send messages</param>
        public AscServiceClient(IAscServerClient serverClient,
            RequestReplyMessenger<IAscServerClient> requestReplyMessenger) {
            _serverClient = serverClient;
            _serverClient.Disconnected += Client_Disconnected;
            _requestReplyMessenger = requestReplyMessenger;
        }

    #endregion

    #region Public methods

        /// <summary>
        ///     Closes client connection.
        /// </summary>
        public void Disconnect() {
            _serverClient.Disconnect();
        }

        /// <summary>
        ///     Gets the client proxy interface that provides calling client methods remotely.
        /// </summary>
        /// <typeparam name="T">Type of client interface</typeparam>
        /// <returns>Client interface</returns>
        public T GetClientProxy<T>(Guid serviceID) where T : class {
            _realProxy = new RemoteInvokeProxy<T, IAscServerClient>(_requestReplyMessenger, serviceID);
            return (T) _realProxy.GetTransparentProxy();
        }

    #endregion

    #region Private methods

        /// <summary>
        ///     Handles disconnect event of _serverClient object.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void Client_Disconnected(object sender, EventArgs e) {
            _requestReplyMessenger.Stop();
            OnDisconnected();
        }

    #endregion

    #region Event raising methods

        /// <summary>
        ///     Raises Disconnected event.
        /// </summary>
        private void OnDisconnected() {
            var handler = Disconnected;
            if (handler != null) handler(this, EventArgs.Empty);
        }

    #endregion
    }
#else
    /// <summary>
    ///     Implements IAscServiceClient.
    ///     It is used to manage and monitor a service client.
    /// </summary>
    internal class AscServiceClient : IAscServiceClient
    {
        #region Public events

        /// <summary>
        ///     This event is raised when this client is disconnected from server.
        /// </summary>
        public event EventHandler Disconnected;

        #endregion

        #region Public properties

        /// <summary>
        ///     Unique identifier for this client.
        /// </summary>
        public long ClientId
        {
            get { return _serverClient.ClientId; }
        }

        /// <summary>
        ///     Gets endpoint of remote application.
        /// </summary>
        public AscEndPoint RemoteEndPoint
        {
            get { return _serverClient.RemoteEndPoint; }
        }

        /// <summary>
        ///     Gets the communication state of the Client.
        /// </summary>
        public CommunicationStates CommunicationState
        {
            get { return _serverClient.CommunicationState; }
        }

        #endregion

        #region Private fields

        /// <summary>
        ///     Reference to underlying IAscServerClient object.
        /// </summary>
        private readonly IAscServerClient _serverClient;

        /// <summary>
        ///     This object is used to send messages to client.
        /// </summary>
        private readonly RequestReplyMessenger<IAscServerClient> _requestReplyMessenger;


        #endregion

        #region Constructor

        /// <summary>
        ///     Creates a new AscServiceClient object.
        /// </summary>
        /// <param name="serverClient">Reference to underlying IAscServerClient object</param>
        /// <param name="requestReplyMessenger">RequestReplyMessenger to send messages</param>
        public AscServiceClient(IAscServerClient serverClient,
            RequestReplyMessenger<IAscServerClient> requestReplyMessenger)
        {
            _serverClient = serverClient;
            _serverClient.Disconnected += Client_Disconnected;
            _requestReplyMessenger = requestReplyMessenger;
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Closes client connection.
        /// </summary>
        public void Disconnect()
        {
            _serverClient.Disconnect();
        }

        /// <summary>
        ///     Gets the client proxy interface that provides calling client methods remotely.
        /// </summary>
        /// <typeparam name="T">Type of client interface</typeparam>
        /// <returns>Client interface</returns>
        public T GetClientProxy<T>(Guid serviceID) where T : class
        {
            var proxy = DispatchProxy.Create<T, RemoteInvokeProxy<T, IAscServerClient>>();
            (proxy as RemoteInvokeProxy<T, IAscServerClient>).Configure(_requestReplyMessenger, serviceID);
            return proxy;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Handles disconnect event of _serverClient object.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void Client_Disconnected(object sender, EventArgs e)
        {
            _requestReplyMessenger.Stop();
            OnDisconnected();
        }

        #endregion

        #region Event raising methods

        /// <summary>
        ///     Raises Disconnected event.
        /// </summary>
        private void OnDisconnected()
        {
            var handler = Disconnected;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion
    }
#endif
}