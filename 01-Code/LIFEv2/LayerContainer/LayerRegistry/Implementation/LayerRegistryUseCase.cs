//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using Hik.Communication.Scs.Client;
using Hik.Communication.Scs.Communication.EndPoints;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;
using Hik.Communication.ScsServices.Service;
using LayerRegistry.Interfaces;
using LifeAPI.Layer;
using LNSConnector.Interface;
using LNSConnector.TransportTypes;
using NodeRegistry.Interface;

namespace LayerRegistry.Implementation {
    internal class LayerRegistryUseCase : ILayerRegistry {
        private IDictionary<Type, ILayer> _localLayers;
        private readonly IScsServiceClient<ILayerNameService> _layerNameServiceClient;
        private readonly ILayerNameService _layerNameService;
        private readonly NodeRegistryConfig _nodeRegistryConfig;
        private readonly List<IScsServiceApplication> _layerServers;
        private int _layerServiceStartPort;

        public LayerRegistryUseCase(INodeRegistry nodeRegistry, NodeRegistryConfig nodeRegistryConfig)
        {
            _nodeRegistryConfig = nodeRegistryConfig;

            _layerServers = new List<IScsServiceApplication>();

            _layerServiceStartPort = nodeRegistryConfig.LayerServiceStartPort;
            TNodeInformation simManager = null;
            while (simManager == null)
            {
                simManager = nodeRegistry.GetAllNodesByType(NodeType.SimulationManager).FirstOrDefault();
                if (simManager != null) continue;
                Console.WriteLine("No SimManager found! Waiting 3 seconds for re-check...");
                // No SimManager found so wait for 3 seconds and recheck
                var manualEvent = new ManualResetEvent(false);
                nodeRegistry.SimulationManagerConnected += (sender, information) =>
                {
                    simManager = information;
                    manualEvent.Set();
                };
                manualEvent.WaitOne(3000);
            }


            // now create layerNameService Stub
            _layerNameServiceClient =
                ScsServiceClientBuilder.CreateClient<ILayerNameService>(
                        "tcp://" + simManager.NodeEndpoint.IpAddress + ":" + simManager.NodeEndpoint.Port
                );

            // connect the client!
            _layerNameServiceClient.Connect();

            _layerNameService = _layerNameServiceClient.ServiceProxy;

            _localLayers = new Dictionary<Type, ILayer>();
        }

        #region ILayerRegistry Members

        public void RemoveLayerInstance(Type layerType) {
            if (!_localLayers.ContainsKey(layerType)) return;
            _layerNameServiceClient.ServiceProxy.RemoveLayer(layerType, new TLayerNameServiceEntry(_nodeRegistryConfig.NodeEndPointIP, _nodeRegistryConfig.NodeEndPointPort, layerType));
            _localLayers.Remove(layerType);
            _layerServiceStartPort--; // TODO: this won't work... need to manage a port array or something...
        }

        public void ResetLayerRegistry() {
            foreach (var localLayer in _localLayers)
            {
                _layerNameServiceClient.ServiceProxy.RemoveLayer(localLayer.GetType(), new TLayerNameServiceEntry(_nodeRegistryConfig.NodeEndPointIP, _nodeRegistryConfig.NodeEndPointPort, localLayer.GetType()));
            }
            _localLayers = new ConcurrentDictionary<Type, ILayer>();
        }

        public void RegisterLayer(ILayer layer) {
            // store in Dict for local usage, by its direct type
            _localLayers.Add(layer.GetType(), layer);
            // and by its direct interface type if any
            if (layer.GetType().GetInterfaces().Length > 0) {
                var infs = layer.GetType().GetInterfaces();
                foreach (var type in infs.Where(type => type.Namespace != null && !type.Namespace.StartsWith("LifeAPI"))) {
                    _localLayers.Add(type, layer);
                }
            }
            // this is necessary in the case somebody is neither using distribution nor interfaces, but still has dependencies


            // check whether this layer should create a service endpoint and be remotely accessible or not
            var layerType = layer.GetType();
            // check if an interface with ScsService Attribute is present
            Type interfaceType = null;
            foreach (var @interface in from @interface in layerType.GetInterfaces() from customAttributeData in @interface.GetTypeInfo().CustomAttributes where customAttributeData.AttributeType == typeof(ScsServiceAttribute) select @interface)
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

                // store LayerRegistryEntry in LayerNameService for remote resolution
                _layerNameServiceClient.ServiceProxy.RegisterLayer(interfaceType, new TLayerNameServiceEntry(_nodeRegistryConfig.NodeEndPointIP, serversPort, layer.GetType()));
            }
        }

        public object GetLayerInstance(Type layerType)
        {
            return _localLayers.ContainsKey(layerType) ? _localLayers[layerType] : GetRemoteLayerInstance(layerType);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Deserializes RegistryEntry, reflects generic method CreateClient from SCS Framework
        ///     and invokes it with parameters fetched from the retreived RegistryEntry.
        ///     The return value is retreived as 'dynamic' to be able to get the ServiceProxy of layerType.
        /// </summary>
        /// <param name="layerType"></param>
        /// <returns>A realproxy implementing the layerType's interface. Return type is object because of C# reflection bullshit. 
        /// DONT. EVER. TOUCH. THIS!</returns>
        private object GetRemoteLayerInstance(Type layerType)
        {
            var entry = _layerNameServiceClient.ServiceProxy.ResolveLayer(layerType);
            var createClientMethod = typeof(ScsServiceClientBuilder).GetMethod("CreateClient", new[] { typeof(ScsEndPoint), typeof(object) });

            // we need to use the layer's interface type and not the class type, so make sure
            // layerType either is an interface type or reflect the correct interface type
            MethodInfo genericCreateClientMethod;
            Type interfaceType = null;
            if (layerType.GetTypeInfo().IsInterface)
            {
                interfaceType = layerType;
                genericCreateClientMethod = createClientMethod.MakeGenericMethod(interfaceType);
            }
            else 
            {
                foreach (var @interface in from @interface in layerType.GetInterfaces() from customAttributeData in @interface.GetTypeInfo().CustomAttributes where customAttributeData.AttributeType == typeof(ScsServiceAttribute) select @interface)
                {
                    interfaceType = @interface;
                }
                genericCreateClientMethod = createClientMethod.MakeGenericMethod(interfaceType);
            }


            var scsStub = genericCreateClientMethod.Invoke(null, new object[] { new ScsTcpEndPoint(entry.IpAddress, entry.Port), null });

            Type typeOfScsStub = scsStub.GetType();

            // set timeout to infinite
            typeOfScsStub.GetProperty("Timeout").SetValue(scsStub, -1);

            // cast to IConnectableClient since dynamic binding only exposes the statically implemented members
            ((IConnectableClient)scsStub).Connect();

            var proxy = typeOfScsStub.GetProperty("ServiceProxy").GetValue(scsStub);

            return proxy;
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