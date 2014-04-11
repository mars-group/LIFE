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
        private KademliaNode _kademliaNode;
        private readonly int _kademliaPort;

        public DistributedKeyValueStoreUseCase(INodeRegistry nodeRegistry) {
            var config = Configuration.GetConfiguration<DistributedKeyValueStoreConfig>();
            _nodeRegistry = nodeRegistry;
            _kademliaPort = config.KademliaPort;
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
                // no other node present. We assume that we are alone and start the DHT ring on our own.
                _kademliaNode.Bootstrap(_kademliaPort);
                // subscribe for new node connected event.
                _nodeRegistry.SubscribeForNewNodeConnected(OnNewNodeConnected);
            }

            // join network
            _kademliaNode.JoinNetwork();

        }

        private void OnNewNodeConnected(NodeInformationType newnode) {
            
            // wait a moment
            Thread.Sleep(50);

            // now check if we still are alone in the ring, if so, try to join the other node
            // since it might have joined another ring
            if (_kademliaNode.GetNodeCount() > 0) return;
            _kademliaNode = new KademliaNode(_kademliaPort);
            _kademliaNode.Bootstrap(new IPEndPoint(IPAddress.Parse(newnode.NodeEndpoint.IpAddress), newnode.NodeEndpoint.Port));
        }
    }
}
