using System;
using System.Reflection;
using ASC.Communication.Scs.Client;
using ASC.Communication.Scs.Communication;
using ASC.Communication.Scs.Communication.Messages;
using ASC.Communication.Scs.Communication.Messengers;
using ASC.Communication.ScsServices.Communication;
using ASC.Communication.ScsServices.Communication.Messages;

namespace ASC.Communication.ScsServices.Client {
    /// <summary>
    ///     Represents a service client that consumes a ACS service.
    /// </summary>
    /// <typeparam name="T">Type of service interface</typeparam>
    internal class AscServiceClient<T> : IAscServiceClient<T> where T : class {
        #region Public events

        /// <summary>
        ///     This event is raised when client connected to server.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        ///     This event is raised when client disconnected from server.
        /// </summary>
        public event EventHandler Disconnected;

        #endregion

        #region Public properties

        /// <summary>
        ///     Timeout for connecting to a server (as milliseconds).
        ///     Default value: 15 seconds (15000 ms).
        /// </summary>
        public int ConnectTimeout {
            get { return _client.ConnectTimeout; }
            set { _client.ConnectTimeout = value; }
        }

        /// <summary>
        ///     Gets the current communication state.
        /// </summary>
        public CommunicationStates CommunicationState {
            get { return _client.CommunicationState; }
        }

        /// <summary>
        ///     Reference to the service proxy to invoke remote service methods.
        /// </summary>
        public T ServiceProxy { get; private set; }

        /// <summary>
        ///     Timeout value when invoking a service method.
        ///     If timeout occurs before end of remote method call, an exception is thrown.
        ///     Use -1 for no timeout (wait indefinite).
        ///     Default value: 60000 (1 minute).
        /// </summary>
        public int Timeout {
            get { return _requestReplyMessenger.Timeout; }
            set { _requestReplyMessenger.Timeout = value; }
        }
        
        #endregion

        #region Private fields

        /// <summary>
        ///     Underlying IScsClient object to communicate with server.
        /// </summary>
        private readonly IScsClient _client;

        /// <summary>
        ///     Messenger object to send/receive messages over _client.
        /// </summary>
        private readonly RequestReplyMessenger<IScsClient> _requestReplyMessenger;

        /// <summary>
        ///     This object is used to create a transparent proxy to invoke remote methods on server.
        /// </summary>
        private readonly AutoConnectRemoteInvokeProxy<T, IScsClient> _realServiceProxy;

        /// <summary>
        ///     The client object that is used to call method invokes in client side.
        ///     May be null if client has no methods to be invoked by server.
        /// </summary>
        private readonly object _clientObject;

        #endregion

        #region Constructor

        /// <summary>
        ///     Creates a new AscServiceClient object.
        /// </summary>
        /// <param name="requestReplyMessenger">Underlying IScsClient object to communicate with server</param>
        /// <param name="clientObject">
        ///     The client object that is used to call method invokes in client side.
        ///     May be null if client has no methods to be invoked by server.
        /// </param>
        /// <param name="serviceID"></param>
        public AscServiceClient(RequestReplyMessenger<IScsClient> requestReplyMessenger, object clientObject, Guid serviceID) {
            _client = requestReplyMessenger.Messenger;
            _clientObject = clientObject;

            _client.Connected += Client_Connected;
            _client.Disconnected += Client_Disconnected;

            _requestReplyMessenger = requestReplyMessenger;
            _requestReplyMessenger.Start();
            //_requestReplyMessenger.MessageReceived += RequestReplyMessenger_MessageReceived;

            _realServiceProxy = new AutoConnectRemoteInvokeProxy<T, IScsClient>(_requestReplyMessenger, this, serviceID);
            ServiceProxy = (T) _realServiceProxy.GetTransparentProxy();
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Connects to server.
        /// </summary>
        public void Connect() {
            _client.Connect();
        }

        /// <summary>
        ///     Disconnects from server.
        ///     Does nothing if already disconnected.
        /// </summary>
        public void Disconnect() {
            _client.Disconnect();
        }

        /// <summary>
        ///     Calls Disconnect method.
        /// </summary>
        public void Dispose() {
            Disconnect();
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Handles Connected event of _client object.
        /// </summary>
        /// <param name="sender">Source of object</param>
        /// <param name="e">Event arguments</param>
        private void Client_Connected(object sender, EventArgs e) {

            OnConnected();
        }

        /// <summary>
        ///     Handles Disconnected event of _client object.
        /// </summary>
        /// <param name="sender">Source of object</param>
        /// <param name="e">Event arguments</param>
        private void Client_Disconnected(object sender, EventArgs e) {
            _requestReplyMessenger.Stop();
            OnDisconnected();
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Raises Connected event.
        /// </summary>
        private void OnConnected() {
            var handler = Connected;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Raises Disconnected event.
        /// </summary>
        private void OnDisconnected() {
            var handler = Disconnected;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion
    }
}