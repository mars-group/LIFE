using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulticastAdapter.Interface
{
    public interface IMulticastAdapter : IMulticastReceiver, IMulticastSender
    {
        /// <summary>
        ///     Close the underlying communication socket
        /// </summary>
        void CloseSocket();


        /// <summary>
        ///     Reopen the closed socket if it was closed before. If the method is called and the Socket is already open nothing
		///     happens.
        /// </summary>
        void ReopenSocket();

    }
}
