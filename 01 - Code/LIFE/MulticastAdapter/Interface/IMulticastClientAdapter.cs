using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MulticastAdapter.Interface
{
    interface IMulticastClientAdapter
    {
        void JoinMulticastGroup(IPAddress ipAddress);
        void LeaveMulticastGroup(IPAddress ipAddress);
        void SendMessageToMulticastGroup(byte[] msg);
        
    }
}
