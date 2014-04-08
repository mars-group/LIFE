using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using CommonTypes.DataTypes;
using ConfigurationAdapter.Interface;
using Daylight;
using DistributedKeyValueStore.Interface;
using DistributedKeyValueStore.Interface.Config;
using NodeRegistry.Interface;

namespace DistributedKeyValueStore.Implementation
{
    class DistributedKeyValueStoreUseCase : IDistributedKeyValueStore
    {
        private readonly INodeRegistry _nodeRegistry;
        private readonly KademliaNode _kademliaNode;
        private readonly int _kademliaPort;

        public DistributedKeyValueStoreUseCase(INodeRegistry nodeRegistry) {
            var path = "./" + typeof(DistributedKeyValueStoreConfig).Name + ".config";
            var config = new Configuration<DistributedKeyValueStoreConfig>(path);
            _nodeRegistry = nodeRegistry;
            _kademliaPort = config.Content.KademliaPort;
            _kademliaNode = new KademliaNode(_kademliaPort, ID.HostID());
            JoinKademliaDht();
        }

        public void Put(string key, string value) {
            throw new System.NotImplementedException();
        }

        public IList<string> Get(string key) {
            throw new System.NotImplementedException();
        }

        private void JoinKademliaDht()
        {
            NodeInformationType otherNode = null;

            // loop and wait until any other node is up and running
            otherNode = _nodeRegistry.GetAllNodes().FirstOrDefault();

            if (otherNode != null) {
                _kademliaNode.Bootstrap(new IPEndPoint(IPAddress.Parse(otherNode.NodeEndpoint.IpAddress),
                    otherNode.NodeEndpoint.Port));

                // wait to fill bucket list
                Thread.Sleep(50);

                // join network
                _kademliaNode.JoinNetwork();
            }
            else {
                _kademliaNode.Bootstrap(_kademliaPort);
            }


        }
    }
}
