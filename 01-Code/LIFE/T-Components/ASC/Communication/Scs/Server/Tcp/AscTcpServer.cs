using ASC.Communication.Scs.Communication.Channels;
using ASC.Communication.Scs.Communication.Channels.Tcp;
using ASC.Communication.Scs.Communication.EndPoints.Tcp;
using ASC.Communication.Scs.Communication.Messengers;

namespace ASC.Communication.Scs.Server.Tcp {
    /// <summary>
    ///     This class is used to create a TCP server.
    /// </summary>
    internal class AscTcpServer : AscServerBase {
        /// <summary>
        ///     The endpoint address of the server to listen incoming connections.
        /// </summary>
        private readonly AscTcpEndPoint _endPoint;

        /// <summary>
        ///     Creates a new AscTcpServer object.
        /// </summary>
        /// <param name="endPoint">The endpoint address of the server to listen incoming connections</param>
        public AscTcpServer(AscTcpEndPoint endPoint) {
            _endPoint = endPoint;
        }

        public override IMessenger GetMessenger() {
            throw new System.NotImplementedException();
        }

        /// <summary>
        ///     Creates a TCP connection listener.
        /// </summary>
        /// <returns>Created listener object</returns>
        protected override IConnectionListener CreateConnectionListener() {
            return new TcpConnectionListener(_endPoint);
        }
    }
}