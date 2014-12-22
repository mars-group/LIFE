using System;
using ASC.Communication.Scs.Communication;
using ASC.Communication.Scs.Communication.EndPoints;

namespace ASC.Communication.ScsServices.Service {
    /// <summary>
    ///     Represents a client that uses a SDS service.
    /// </summary>
    public interface IScsServiceClient {
        /// <summary>
        ///     This event is raised when client is disconnected from service.
        /// </summary>
        event EventHandler Disconnected;

        /// <summary>
        ///     Unique identifier for this client.
        /// </summary>
        long ClientId { get; }

        /// <summary>
        ///     Gets endpoint of remote application.
        /// </summary>
        ScsEndPoint RemoteEndPoint { get; }

        /// <summary>
        ///     Gets the communication state of the Client.
        /// </summary>
        CommunicationStates CommunicationState { get; }

        /// <summary>
        ///     Closes client connection.
        /// </summary>
        void Disconnect();

        /// <summary>
        ///     Gets the client proxy interface that provides calling client methods remotely.
        /// </summary>
        /// <param name="serviceID">The ID of the ServiceObject on the Server</param>
        /// <typeparam name="T">Type of client interface</typeparam>
        /// <returns>Client interface</returns>
        T GetClientProxy<T>(Guid serviceID) where T : class;
    }
}