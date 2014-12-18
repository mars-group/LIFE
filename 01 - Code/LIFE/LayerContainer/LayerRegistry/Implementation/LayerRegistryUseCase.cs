﻿// /*******************************************************
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
using CommonTypes.Types;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;
using LayerRegistry.Interfaces;
using LifeAPI.Layer;
using LNSConnector.Interface;
using LNSConnector.TransportTypes;
using NodeRegistry.Exceptions;
using NodeRegistry.Interface;

namespace LayerRegistry.Implementation {
    internal class LayerRegistryUseCase : ILayerRegistry {
        private readonly IDictionary<Type, ILayer> _localLayers;
        private readonly ILayerNameService _layerNameService;
        private readonly NodeRegistryConfig _nodeRegistryConfig;

        public LayerRegistryUseCase(INodeRegistry nodeRegistry, NodeRegistryConfig nodeRegistryConfig)
        {
            _nodeRegistryConfig = nodeRegistryConfig;

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
        }

        public void ResetLayerRegistry() {
            throw new NotImplementedException();
        }

        public ILayer GetLayerInstance(Type layerType)
        {
            return _localLayers.ContainsKey(layerType) ? _localLayers[layerType] : GetRemoteLayerInstance(layerType);
        }


        public void RegisterLayer(ILayer layer) {
            // store in Dict for local usage
            _localLayers.Add(layer.GetType(), layer);

            // store LayerRegistryEntry in DHT for remote usage
            _layerNameService.RegisterLayer(layer.GetType(),new TLayerNameServiceEntry(_nodeRegistryConfig.NodeEndPointIP,_nodeRegistryConfig.NodeEndPointPort , layer.GetType()));
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