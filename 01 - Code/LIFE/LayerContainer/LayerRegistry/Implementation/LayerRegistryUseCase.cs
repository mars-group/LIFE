// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 09.07.2014
//  *******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DistributedKeyValueStore.Interface;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;
using LayerRegistry.Interfaces;
using LayerRegistry.Interfaces.Config;
using LifeAPI.Layer;
using Newtonsoft.Json;

namespace LayerRegistry.Implementation {
    internal class LayerRegistryUseCase : ILayerRegistry {
        private readonly IDistributedKeyValueStore _distributedKeyValueStore;
        private readonly LayerRegistryConfig _layerRegistryConfig;
        private readonly string _ownIpAddress;
        private readonly int _ownPort;
        private readonly IDictionary<Type, ILayer> _localLayers;

        public LayerRegistryUseCase
            (IDistributedKeyValueStore distributedKeyValueStore, LayerRegistryConfig layerRegistryConfig) {
            _distributedKeyValueStore = distributedKeyValueStore;
            _layerRegistryConfig = layerRegistryConfig;

            _localLayers = new Dictionary<Type, ILayer>();

            _ownIpAddress = _layerRegistryConfig.MainNetworkAddress;
            _ownPort = _layerRegistryConfig.MainNetworkPort;
        }

        #region ILayerRegistry Members

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
            string value = JsonConvert.SerializeObject(new LayerRegistryEntry(_ownIpAddress, _ownPort, layer.GetType()));
            _distributedKeyValueStore.Put(layer.GetType().ToString(), JsonConvert.SerializeObject(value));
        }

        #endregion

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

            LayerRegistryEntry entry = JsonConvert.DeserializeObject<LayerRegistryEntry>(registryEntries.First());

            MethodInfo createClientMethod = typeof (ScsServiceClientBuilder).GetMethod("CreateClient");
            MethodInfo genericCreateClientMethod = createClientMethod.MakeGenericMethod(layerType);
            dynamic scsStub = genericCreateClientMethod.Invoke
                (null,
                    new[] {new ScsTcpEndPoint(entry.IpAddress, entry.Port)});

            return scsStub.ServiceProxy;
        }

        #endregion
    }

    [Serializable]
    internal class LayerInstanceNotRegisteredException : Exception {}
}