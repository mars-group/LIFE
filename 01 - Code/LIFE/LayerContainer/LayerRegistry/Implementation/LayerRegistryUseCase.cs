using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using CommonTypes.DataTypes;
using ConfigurationAdapter.Implementation;
using ConfigurationAdapter.Interface;
using Daylight;
using DistributedKeyValueStore.Interface;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;
using LayerAPI.Interfaces;
using LayerRegistry.Interfaces;
using Newtonsoft.Json;
using NodeRegistry.Interface;

namespace LayerRegistry.Implementation {
    internal class LayerRegistryUseCase : ILayerRegistry {
        private readonly IDistributedKeyValueStore _distributedKeyValueStore;
        private readonly string _ownIpAddress;
        private readonly int _ownPort;
        private readonly IDictionary<Type, ILayer> _localLayers;


        public LayerRegistryUseCase(IDistributedKeyValueStore distributedKeyValueStore) {
            _distributedKeyValueStore = distributedKeyValueStore;
            _localLayers = new Dictionary<Type, ILayer>();

            IConfigurationAdapter configurationAdapter = new AppSettingAdapterImpl();

            _ownIpAddress = configurationAdapter.GetValue("MainNetworkAddress");
            _ownPort = configurationAdapter.GetInt32("MainNetworkPort");
        }

        public ILayer RemoveLayerInstance(Type layerType) {
            throw new NotImplementedException();
        }

        public void ResetLayerRegistry() {
            throw new NotImplementedException();
        }

        public ILayer GetRemoteLayerInstance(Type layerType) {
            if (_localLayers.ContainsKey(layerType)) return _localLayers[layerType];
            return GetRemoteLayerInstance(_distributedKeyValueStore.Get(layerType.Name), layerType);
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