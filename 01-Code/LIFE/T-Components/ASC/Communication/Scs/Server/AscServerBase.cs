using ASC.Communication.Scs.Communication.Channels;
using ASC.Communication.Scs.Communication.Messengers;
using ASC.Communication.Scs.Communication.Protocols;
using CustomUtilities.Collections;

namespace ASC.Communication.Scs.Server {
    /// <summary>
    ///     This class provides base functionality for server classes.
    /// </summary>
    internal abstract class AscServerBase : IAscServer {

        #region Public properties

        /// <summary>
        ///     Gets/sets wire protocol that is used while reading and writing messages.
        /// </summary>
        public IAcsWireProtocolFactory WireProtocolFactory { get; set; }

        /// <summary>
        ///     A collection of clients that are connected to the server.
        /// </summary>
        public ThreadSafeSortedList<long, IAscServerClient> Clients { get; private set; }

        #endregion

        #region Private properties

        /// <summary>
        ///     This object is used to listen incoming connections.
        /// </summary>
        private IConnectionListener _connectionListener;

        #endregion

        #region Constructor

        /// <summary>
        ///     Constructor.
        /// </summary>
        protected AscServerBase() {
            Clients = new ThreadSafeSortedList<long, IAscServerClient>();
            WireProtocolFactory = WireProtocolManager.GetDefaultWireProtocolFactory();
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Starts the server.
        /// </summary>
        public virtual void Start() {
            
        }

        /// <summary>
        ///     Stops the server.
        /// </summary>
        public virtual void Stop() {
        }

        public abstract IMessenger GetMessenger();

        #endregion

        #region Protected abstract methods

        /// <summary>
        ///     This method is implemented by derived classes to create appropriate connection listener to listen incoming
        ///     connection requets.
        /// </summary>
        /// <returns></returns>
        protected abstract IConnectionListener CreateConnectionListener();

        #endregion

        #region Private methods


        #endregion
    }
}