using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using AppSettingsManager.Interface;
using CommonTypes.DataTypes;
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
            _kademliaNode.Put(ID.Hash(key), value);
        }

        public IList<string> Get(string key) {
            return _kademliaNode.Get(ID.Hash(key));
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


            }
            else {
                _kademliaNode.Bootstrap(_kademliaPort);
            }

            // join network
            _kademliaNode.JoinNetwork();

        }
    }
}
