// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 09.07.2014
//  *******************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommonTypes.Types;
using Hik.Communication.Scs.Communication.EndPoints;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;
using Hik.Communication.ScsServices.Service;
using LayerRegistry.Interfaces;
using LifeAPI.Layer;
using LNSConnector.Interface;
using LNSConnector.TransportTypes;
using NodeRegistry.Exceptions;
using NodeRegistry.Interface;

namespace LayerRegistry.Implementation {
    internal class LayerRegistryUseCase : ILayerRegistry {
        private IDictionary<Type, ILayer> _localLayers;
        private readonly ILayerNameService _layerNameService;
        private readonly NodeRegistryConfig _nodeRegistryConfig;
        private List<IScsServiceApplication> _layerServers;
        private int _layerServiceStartPort;

        public LayerRegistryUseCase(INodeRegistry nodeRegistry, NodeRegistryConfig nodeRegistryConfig)
        {
            _nodeRegistryConfig = nodeRegistryConfig;

            _layerServers = new List<IScsServiceApplication>();

            _layerServiceStartPort = nodeRegistryConfig.LayerServiceStartPort;

            // fetch SimulationManager Node from registry
            var simManager = nodeRegistry.GetAllNodesByType(NodeType.SimulationManager).FirstOrDefault();
            if (simManager == null)
            {
                throw new NoSimulationManagerPresentException("No SimulationManager is connected, please check your network configuration.");
            }

            // now create layerNameService Stub
            _layerNameService =
                ScsServiceClientBuilder.CreateClient<ILayerNameService>(
                        new ScsTcpEndPoint(simManager.NodeEndpoint.IpAddress, simManager.NodeEndpoint.Port)
                ).ServiceProxy;

            _localLayers = new Dictionary<Type, ILayer>();
        }

        #region ILayerRegistry Members

        public void RemoveLayerInstance(Type layerType) {
            if (!_localLayers.ContainsKey(layerType)) return;
            _layerNameService.RemoveLayer(layerType, new TLayerNameServiceEntry(_nodeRegistryConfig.NodeEndPointIP, _nodeRegistryConfig.NodeEndPointPort, layerType));
            _localLayers.Remove(layerType);
            _layerServiceStartPort--;
        }

        public void ResetLayerRegistry() {
            foreach (var localLayer in _localLayers)
            {
                _layerNameService.RemoveLayer(localLayer.GetType(), new TLayerNameServiceEntry(_nodeRegistryConfig.NodeEndPointIP, _nodeRegistryConfig.NodeEndPointPort, localLayer.GetType()));
            }
            _localLayers = new ConcurrentDictionary<Type, ILayer>();
        }

        public ILayer GetLayerInstance(Type layerType)
        {
            return _localLayers.ContainsKey(layerType) ? _localLayers[layerType] : GetRemoteLayerInstance(layerType);
        }


        public void RegisterLayer(ILayer layer) {
            // store in Dict for local usage
            _localLayers.Add(layer.GetType(), layer);

            // check whether this layer should create a service endpoint and be remotely accessible or not
            
            var layerType = layer.GetType();
            // get first interface, is always the directly implemented interface
            Type interfaceType = null;
            foreach (var @interface in from @interface in layerType.GetInterfaces() from customAttributeData in @interface.CustomAttributes where customAttributeData.AttributeType == typeof(ScsServiceAttribute) select @interface)
            {
                interfaceType = @interface;
            }

            // if interfaceType is not null, we can create a service endpoint, so do it.
            if (interfaceType != null) {
                // add service to SCS Server
                var serversPort = _layerServiceStartPort++;


                var server = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(serversPort));
                var addServiceMethod = server.GetType().GetMethod("AddService");
                var genericAddServiceMethod = addServiceMethod.MakeGenericMethod(interfaceType, layerType);
                genericAddServiceMethod.Invoke(server, new object[]{layer});

                server.Start();
                _layerServers.Add(server);

                // store LayerRegistryEntry in DHT for remote usage
                _layerNameService.RegisterLayer(layer.GetType(), new TLayerNameServiceEntry(_nodeRegistryConfig.NodeEndPointIP, serversPort, layer.GetType()));
            }
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
        private ILayer GetRemoteLayerInstance(Type layerType)
        {
            var entry = _layerNameService.ResolveLayer(layerType);
            MethodInfo createClientMethod = typeof(ScsServiceClientBuilder).GetMethod("CreateClient", new[] { typeof(ScsEndPoint), typeof(object) });
            MethodInfo genericCreateClientMethod = createClientMethod.MakeGenericMethod(layerType);
            dynamic scsStub = genericCreateClientMethod.Invoke
                (null,
                    new[] {new ScsTcpEndPoint(entry.IpAddress, entry.Port)});

            return scsStub.ServiceProxy;
        }

        #endregion
    }

    [Serializable]
    internal class SCSServiceAttributeHasNotBeenSpecifiedException : Exception
    {
        public SCSServiceAttributeHasNotBeenSpecifiedException(string msg) : base(msg){}
    }


    [Serializable]
    internal class LayerInstanceNotRegisteredException : Exception {}
}