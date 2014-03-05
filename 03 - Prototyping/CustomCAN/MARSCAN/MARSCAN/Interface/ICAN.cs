using System;

namespace MARSCAN.Interface
{
    /// <summary>
    /// A generic callback that is able to receive messages deriving from or equal to T.
    /// </summary>
    /// <typeparam name="M">The message's type.</typeparam>
    /// <param name="message">The message.</param>
    public delegate void Receiver<M>(M message);

    /// <summary>
    /// This interface is the entry point for communication with the MARSCAN network.
    /// </summary>
    public interface ICAN
    {
        /// <summary>
        /// Registers a listener for incoming messages
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="messageReceiver"></param>
        void Register<M>(Receiver<M> messageReceiver);

        /// <summary>
        /// Starts the supposedly first node of the P2P network. It will be in a passive node, waiting for connections of other peers.
        /// </summary>
        void Bootstrap();

        /// <summary>
        /// Connects this node to the MARSCAN network.
        /// </summary>
        /// <param name="peer"></param>
        void Connect(Uri peer);

        /// <summary>
        /// Create a multicast group with the given name as identifier.
        /// </summary>
        /// <param name="name"></param>
        void CreateGroup(string groupName);

        /// <summary>
        /// Joins a multicast group. Unless the group is left, all messages from that group will be received.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="groupName"></param>
        /// <param name="receiver"></param>
        void JoinGroup<M>(string groupName, Receiver<M> receiver);

        /// <summary>
        /// Leaves a multicastgroup. Messages to that group will from now on not be delivered again.
        /// </summary>
        /// <param name="name"></param>
        void LeaveGroup(string groupName);

        /// <summary>
        /// Publishes the message of type T to the group with the given name (if it exists).
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="groupName"></param>
        /// <param name="message"></param>
        void Publish<M>(string groupName, M message);

        /// <summary>
        /// Broadcasts the message to all nodes in the MARSCAN network.
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="message"></param>
        void Broadcast<M>(M message);
    }
}
