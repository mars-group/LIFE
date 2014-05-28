using System;
using System.Collections.Generic;
using System.Linq;
using DistributedKeyValueStore.Interface;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;
using LayerAPI.Interfaces;
using LayerRegistry.Interfaces;
using LayerRegistry.Interfaces.Config;
using Newtonsoft.Json;


namespace LayerRegistry.Implementation {


    internal class LayerRegistryUseCase : ILayerRegistry {
        private readonly IDistributedKeyValueStore _distributedKeyValueStore;
        private readonly LayerRegistryConfig _layerRegistryConfig;
        private readonly string _ownIpAddress;
        private readonly int _ownPort;
        private readonly IDictionary<Type, ILayer> _localLayers;

        public LayerRegistryUseCase(IDistributedKeyValueStore distributedKeyValueStore, LayerRegistryConfig layerRegistryConfig) {
            _distributedKeyValueStore = distributedKeyValueStore;
            _layerRegistryConfig = layerRegistryConfig;

            _localLayers = new Dictionary<Type, ILayer>();

            _ownIpAddress = _layerRegistryConfig.MainNetworkAddress;
            _ownPort = _layerRegistryConfig.MainNetworkPort;
        }

        public ILayer RemoveLayerInstance(Type layerType) {
            throw new NotImplementedException();
        }

        public void ResetLayerRegistry() {
            throw new NotImplementedException();
        }

        public ILayer GetLayerInstance(Type layerType) {
            if (_localLayers.ContainsKey(layerType)) return _localLayers[layerType];
            return GetRemoteLayerInstance(_distributedKeyValueStore.Get(layerType.ToString()), layerType);
        }


        public void RegisterLayer(ILayer layer) {
            // store in Dict for local usage
            _localLayers.Add(layer.GetType(), layer);

            // store LayerRegistryEntry in DHT for remote usage
            var value = JsonConvert.SerializeObject(new LayerRegistryEntry(_ownIpAddress, _ownPort, layer.GetType()));
            _distributedKeyValueStore.Put(layer.GetType().ToString(), JsonConvert.SerializeObject(value));
        }

        #region Private Methods

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

        #endregion
    }

    internal class LayerInstanceNotRegisteredException : Exception {}
}