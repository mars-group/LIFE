namespace MulticastAdapter.Interface
{
    /// <summary>
    /// Sends byte messages to a configured multicastgroup. The Adapter can be configured to send on multible networkinterfaces which are able to multicast.
    /// </summary>
    public interface IMulticastSender
    {
        /// <summary>
        ///     sends a message to the configured multicastgroup on the configured Multicastinterfaces
        ///     </summary>
        /// <param name="msg">the byte message</param>
        void SendMessageToMulticastGroup(byte[] msg);

        /// <summary>
        ///     Close the underlying communication socket
        /// </summary>
        void CloseSocket();

        /// <summary>
        ///     Reopen the closed socket if it was closed before. If the method is called and the Socket is already open nothing
        ///     happend.
        /// </summary>
        void ReopenSocket();
    }
}