using System;
using ASC.Communication.Scs.Communication;
using ASC.Communication.Scs.Communication.EndPoints;
using ASC.Communication.Scs.Communication.Messengers;

namespace ASC.Communication.Scs.Server {
    /// <summary>
    ///     Represents a client from a perspective of a server.
    /// </summary>
    public interface IAscServerClient : IMessenger {
        /// <summary>
        ///     This event is raised when client disconnected from server.
        /// </summary>
        event EventHandler Disconnected;

        /// <summary>
        ///     Unique identifier for this client in server.
        /// </summary>
        long ClientId { get; }

        /// <summary>
        ///     Gets endpoint of remote application.
        /// </summary>
        AscEndPoint RemoteEndPoint { get; }

        /// <summary>
        ///     Gets the current communication state.
        /// </summary>
        CommunicationStates CommunicationState { get; }

        /// <summary>
        ///     Disconnects from server.
        /// </summary>
        void Disconnect();
    }
}