namespace MulticastAdapter.Interface
{
    public interface IMulticastReciever
    {

        /// <summary>
        /// Listen for new UDP-Multicast messages on the configured port. This Method is blocking.
        /// </summary>
        /// <returns></returns>
        byte[] readMulticastGroupMessage();
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