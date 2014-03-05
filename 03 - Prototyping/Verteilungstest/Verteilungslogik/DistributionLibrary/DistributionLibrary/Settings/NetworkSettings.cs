using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistributionLibrary.Settings
{
    public class NetworkSettings
    {
        public string HostingIP { get; private set; }

        public int HostingPort { get; private set; }

        public bool IsServer { get; private set; }

        public NetworkSettings(string hostingIp, int hostingPort, bool isServer)
        {
            HostingIP = hostingIp;
            HostingPort = hostingPort;
            IsServer = isServer;
        }
    }
}
