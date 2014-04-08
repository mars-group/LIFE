using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using CommonTypes.DataTypes;
using ConfigurationAdapter.Interface;
using Daylight;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;
using LayerAPI.Interfaces;
using LayerRegistry.Interfaces;
using LayerRegistry.Interfaces.Config;
using Newtonsoft.Json;
using NodeRegistry.Interface;

namespace LayerRegistry.Implementation {
    internal class LayerRegistryUseCase : ILayerRegistry {
        private readonly INodeRegistry _nodeRegistry;
        private readonly int _kademliaPort;
        private readonly KademliaNode _kademliaNode;
        private readonly string _ownIpAddress;
        private readonly int _ownPort;
        private readonly IDictionary<Type, ILayer> _localLayers;
        private Configuration<LayerRegistryConfig> _layerRegisterConfig;

        public LayerRegistryUseCase(INodeRegistry nodeRegistry) {

            var path = "./" + typeof(LayerRegistryConfig).Name + ".config";

            _layerRegisterConfig = new Configuration<LayerRegistryConfig>(path);
            _nodeRegistry = nodeRegistry;

            _localLayers = new Dictionary<Type, ILayer>();

            _kademliaPort = _layerRegisterConfig.Content.KademliaPort;
            _ownIpAddress = _layerRegisterConfig.Content.MainNetworkAddress;
            _ownPort = _layerRegisterConfig.Content.MainNetworkPort;
            _kademliaNode = new KademliaNode(_kademliaPort, ID.HostID());
            JoinKademliaDHT();
        }

        public ILayer RemoveLayerInstance(Type layerType) {
            throw new NotImplementedException();
        }

        public void ResetLayerRegistry() {
            throw new NotImplementedException();
        }

        public ILayer GetRemoteLayerInstance(Type layerType) {
            if (_localLayers.ContainsKey(layerType)) return _localLayers[layerType];
            return GetRemoteLayerInstance(GetFromDht(layerType), layerType);
        }


        public void RegisterLayer(ILayer layer) {
            // store in Dict for local usage
            _localLayers.Add(layer.GetType(), layer);

            // store LayerRegistryEntry in DHT for remote usage
            PutIntoDht(layer.GetType());
        }

        #region Private Methods

        private ICollection<string> GetFromDht(Type layerType) {
            return _kademliaNode.Get(ID.Hash(layerType.Name));
        }

        private void PutIntoDht(Type layerType) {
            var value = JsonConvert.SerializeObject(new LayerRegistryEntry(_ownIpAddress, _ownPort, layerType));
            _kademliaNode.Put(ID.Hash(layerType.ToString()), JsonConvert.SerializeObject(value));
        }

        /// <summary>
        ///     Deserializes RegistryEntry, reflects generic method CreateClient from SCS Framework
        ///     and invokes it with parameters fetched from the retreived RegistryEntry.
        ///     The return value is retreived as 'dynamic' to be able to get the ServiceProxy of layerType.
        /// </summary>
        /// <param name="registryEntries"></param>
        /// <param name="layerType"></param>
        /// <returns></returns>
        private static ILayer GetRemoteLayerInstance(ICollection<string> registryEntries, Type layerType) {
            if (registryEntries.Count <= 0) throw new LayerInstanceNotRegisteredException();

            var entry = JsonConvert.DeserializeObject<LayerRegistryEntry>(registryEntries.First());

            var createClientMethod = typeof (ScsServiceClientBuilder).GetMethod("CreateClient");
            var genericCreateClientMethod = createClientMethod.MakeGenericMethod(layerType);
            dynamic scsStub = genericCreateClientMethod.Invoke(null,
                new[] {new ScsTcpEndPoint(entry.IpAddress, entry.Port)});

            return scsStub.ServiceProxy;
        }

        private void JoinKademliaDHT() {
            NodeInformationType otherNode = null;

            // loop and wait until any other node is up and running
            Thread.Sleep(100);
            otherNode = _nodeRegistry.GetAllNodes().FirstOrDefault();

            _kademliaNode.Bootstrap(
                new IPEndPoint(IPAddress.Parse(otherNode.NodeEndpoint.IpAddress),
                    otherNode.NodeEndpoint.Port)
                );

            // wait to fill bucket list
            Thread.Sleep(50);

            // join network
            _kademliaNode.JoinNetwork();
        }

        #endregion
    }

    internal class LayerInstanceNotRegisteredException : Exception {}
}