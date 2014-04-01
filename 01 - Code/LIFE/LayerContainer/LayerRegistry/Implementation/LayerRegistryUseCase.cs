using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using CommonTypes.DataTypes;
using CommonTypes.TransportTypes.SimulationControl;
using CommonTypes.Types;
using Daylight;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.Scs.Server;
using Hik.Communication.ScsServices.Client;
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
		private readonly IDictionary<Type, ILayer> _localLayers;

        public LayerRegistryUseCase(INodeRegistry nodeRegistry)
        {
            _nodeRegistry = nodeRegistry;

			_localLayers = new Dictionary<Type, ILayer> ();

            _kademliaPort = int.Parse(ConfigurationManager.AppSettings.Get("KademliaPort"));
            _ownIpAddress = ConfigurationManager.AppSettings.Get("MainNetworkAddress");
            _ownPort = int.Parse(ConfigurationManager.AppSettings.Get("MainNetworkPort"));
            _kademliaNode = new KademliaNode(_kademliaPort);
            JoinKademliaDHT();
        }

        public ILayer RemoveLayerInstance(Type layerType)
        {
			throw new NotImplementedException();
        }

        public void ResetLayerRegistry()
        {
            throw new NotImplementedException();
        }

        public ILayer GetRemoteLayerInstance(Type layerType)
        {
			if (_localLayers.ContainsKey (layerType)) {
				return _localLayers [layerType];
			} else {
				return GetRemoteLayerInstance (GetFromDHT (layerType), layerType);
			}
        }


        public void RegisterLayer(ILayer layer)
        {
			// store in Dict for local usage
			_localLayers.Add(layer.GetType(), layer);

			// store LayerRegistryEntry in DHT for remote usage
			PutIntoDHT(layer.GetType());
        }



        #region Private Methods
        private ICollection<string> GetFromDHT(Type layerType)
        {
            return _kademliaNode.Get(ID.Hash(layerType.Name));
        }

        private void PutIntoDHT(Type layerType)
        {
            var value = JsonConvert.SerializeObject(new LayerRegistryEntry(_ownIpAddress, _ownPort, layerType));
            _kademliaNode.Put(ID.Hash(layerType.ToString()), JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// Deserializes RegistryEntry, reflects generic method CreateClient from SCS Framework
        /// and invokes it with parameters fetched from the retreived RegistryEntry.
        /// The return value is retreived as 'dynamic' to be able to get the ServiceProxy of layerType.
        /// </summary>
        /// <param name="registryEntries"></param>
        /// <param name="layerType"></param>
        /// <returns></returns>
        private static ILayer GetRemoteLayerInstance(ICollection<string> registryEntries, Type layerType)
        {
            if (registryEntries.Count <= 0) throw new LayerInstanceNotRegisteredException();

            var entry = JsonConvert.DeserializeObject<LayerRegistryEntry>(registryEntries.First());

            var createClientMethod = typeof(ScsServiceClientBuilder).GetMethod("CreateClient");
            var genericCreateClientMethod = createClientMethod.MakeGenericMethod(layerType);
            dynamic scsStub = genericCreateClientMethod.Invoke(null, new[] { new ScsTcpEndPoint(entry.IpAddress, entry.Port) });

            return scsStub.ServiceProxy;
        }

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

    internal class LayerInstanceNotRegisteredException : Exception
    {
    }
}
