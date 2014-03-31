using System;
using System.Configuration;
using System.Linq;
using System.Net;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using Daylight;
using NodeRegistry.Interface;


namespace LayerRegistry.Implementation
{

    using LayerAPI.Interfaces;

    using Interfaces;

    class LayerRegistryUseCase : ILayerRegistry
    {
        private readonly INodeRegistry _nodeRegistry;
        private readonly int _kademliaPort;
        private KademliaNode _kademliaNode;

        public LayerRegistryUseCase(INodeRegistry nodeRegistry)
        {
            _nodeRegistry = nodeRegistry;
            _kademliaPort = int.Parse(ConfigurationManager.AppSettings.Get("KademliaPort"));
            _kademliaNode = new KademliaNode(_kademliaPort);
            JoinKademliaDHT();
        }

        public ILayer RemoveLayerInstance(Guid layerID)
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

        public void RegisterLayer(ILayer layer)
        {
            throw new NotImplementedException();
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
