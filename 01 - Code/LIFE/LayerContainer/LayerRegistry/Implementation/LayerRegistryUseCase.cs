using System;
using System.Configuration;
using System.Linq;
using System.Net;
using CommonTypes.DataTypes;
using CommonTypes.TransportTypes.SimulationControl;
using CommonTypes.Types;
using Daylight;
using Newtonsoft.Json;
using NodeRegistry.Interface;


namespace LayerRegistry.Implementation
{

    using LayerAPI.Interfaces;

    using Interfaces;

    class LayerRegistryUseCase : ILayerRegistry
    {
        private readonly INodeRegistry _nodeRegistry;
        private readonly int _kademliaPort;
        private readonly KademliaNode _kademliaNode;
        private readonly string _ownIpAddress;
        private readonly int _ownPort;

        public LayerRegistryUseCase(INodeRegistry nodeRegistry)
        {
            _nodeRegistry = nodeRegistry;
            _kademliaPort = int.Parse(ConfigurationManager.AppSettings.Get("KademliaPort"));
            _ownIpAddress = ConfigurationManager.AppSettings.Get("MainNetworkAddress");
            _ownPort = int.Parse(ConfigurationManager.AppSettings.Get("MainNetworkPort"));
            _kademliaNode = new KademliaNode(_kademliaPort);
            JoinKademliaDHT();
        }

        public ILayer RemoveLayerInstance(LayerInstanceIdType layerInstanceId)
        {
            throw new NotImplementedException();
        }

        public ILayer RemoveLayerInstance(Type layerType)
        {
            throw new NotImplementedException();
        }

        public void ResetLayerRegistry()
        {
            throw new NotImplementedException();
        }

        public ILayer GetLayerInstance(Type parameterType)
        {
            throw new NotImplementedException();
        }

        public ILayer GetLayerInstance(LayerInstanceIdType layerInstanceId)
        {
            var registryEntries = _kademliaNode.Get(ID.Hash(layerInstanceId.ToString()));
            if (registryEntries.Count > 0)
            {
                var entry = JsonConvert.DeserializeObject<LayerRegistryEntry>(registryEntries.First());
              //  new RemoteInvoke
            }
            return null;
        }

        public void RegisterLayer(ILayer layer)
        {
            var value = JsonConvert.SerializeObject(new LayerRegistryEntry(_ownIpAddress, _ownPort, layer.GetType()));
            _kademliaNode.Put(ID.Hash(layer.GetID().ToString()), JsonConvert.SerializeObject(value));
        }

        #region Private Methods

        private void JoinKademliaDHT()
        {
            NodeInformationType otherNode = null;

            // loop and wait until any other node is up and running
            while (otherNode == null)
            {
                System.Threading.Thread.Sleep(100);
                otherNode = _nodeRegistry.GetAllNodes().FirstOrDefault();    
            }

            _kademliaNode.Bootstrap(
                    new IPEndPoint(IPAddress.Parse(otherNode.NodeEndpoint.IpAddress),
                    otherNode.NodeEndpoint.Port)
                    );

            // wait to fill bucket list
            System.Threading.Thread.Sleep(50);

            // join network
            _kademliaNode.JoinNetwork();
        }

        #endregion
    }
}
