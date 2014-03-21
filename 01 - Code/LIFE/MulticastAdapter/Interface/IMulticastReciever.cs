using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulticastAdapter.Interface
{
    interface IMulticastReciever
    {
        byte[] readMulticastGroupMessage();
        void CloseSocket();
    }
}
