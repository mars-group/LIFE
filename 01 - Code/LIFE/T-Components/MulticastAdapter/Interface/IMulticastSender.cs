namespace MulticastAdapter.Interface
{
    public interface IMulticastSender
    {


        /// <summary>
        ///     sends a message to the multicastgroup
        /// </summary>
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