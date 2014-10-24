﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Hik.Threading;
using LayerAPI.Interfaces;
using LCConnector.TransportTypes;
using RTEManager.Interfaces;

namespace RTEManager.Implementation {
    internal class RTEManagerUseCase : IRTEManager {

        // the tickClients being executed per Layer
        private readonly IDictionary<ILayer, ConcurrentDictionary<ITickClient, byte>> _tickClientsPerLayer;

        // the tickClients which are marked to be deregistered per Layer
        private readonly IDictionary<ILayer, ConcurrentBag<ITickClient>> _tickClientsMarkedForDeletionPerLayer;

        // the tickClients which are marked to be registered per Layer during active simulation
        private readonly IDictionary<ILayer, ConcurrentBag<ITickClient>> _tickClientsMarkedForRegistrationPerLayer;

        // all the layers mapped by their instanceID
        private readonly IDictionary<TLayerInstanceId, ILayer> _layers;

        // indicator whether this Layercontainer ist currently executing a Tick
        private bool _isRunning;

        public RTEManagerUseCase() {
            
            _tickClientsPerLayer = new Dictionary<ILayer, ConcurrentDictionary<ITickClient, byte>>();
            _tickClientsMarkedForDeletionPerLayer = new Dictionary<ILayer, ConcurrentBag<ITickClient>>();
            _tickClientsMarkedForRegistrationPerLayer = new Dictionary<ILayer, ConcurrentBag<ITickClient>>();
            _layers = new Dictionary<TLayerInstanceId, ILayer>();
            _isRunning = false;
        }

        #region Public Methods

        public void RegisterLayer(TLayerInstanceId layerInstanceId, ILayer layer) {
            if (!_tickClientsPerLayer.ContainsKey(layer))
            {
                _tickClientsPerLayer.Add(layer, new ConcurrentDictionary<ITickClient, byte>());
                _tickClientsMarkedForDeletionPerLayer.Add(layer, new ConcurrentBag<ITickClient>());
                _tickClientsMarkedForRegistrationPerLayer.Add(layer, new ConcurrentBag<ITickClient>());
            }
            if (!_layers.ContainsKey(layerInstanceId))
            {
                _layers.Add(layerInstanceId, layer);
            }
        }

        public void UnregisterLayer(TLayerInstanceId layerInstanceId) {
            _tickClientsPerLayer.Remove(_layers[layerInstanceId]);
            _layers.Remove(layerInstanceId);
        }

        public void UnregisterTickClient(ILayer layer, ITickClient tickClient) {
            if (_tickClientsMarkedForDeletionPerLayer.ContainsKey(layer))
            {
                _tickClientsMarkedForDeletionPerLayer[layer].Add(tickClient);
            }
        }

        public void RegisterTickClient(ILayer layer, ITickClient tickClient) {
            if (!_isRunning) {
                _tickClientsPerLayer[layer].TryAdd(tickClient, new byte());
            }
            else {
                _tickClientsMarkedForRegistrationPerLayer[layer].Add(tickClient);
            }
        }

        public void InitializeLayer(TLayerInstanceId instanceId, TInitData initData) {
            _layers[instanceId].InitLayer<TInitData>(initData, RegisterTickClient, UnregisterTickClient);
        }

        public IEnumerable<ITickClient> GetAllTickClientsByLayer(TLayerInstanceId layer) {
            return _tickClientsPerLayer[_layers[layer]].Keys;
        }

        public long AdvanceOneTick() {
            _isRunning = true;
            var stopWatch = Stopwatch.StartNew();


            Parallel.ForEach(
                _tickClientsPerLayer.Keys,
                layer => Parallel.ForEach(_tickClientsPerLayer[layer],
                    client => client.Key.Tick()
                    )
                );




            // clean up all deleted tickClients
            Parallel.ForEach
                (
                _tickClientsMarkedForDeletionPerLayer.Keys,
                layer => Parallel.ForEach(_tickClientsMarkedForDeletionPerLayer[layer],
                    tickClientToBeRemoved => {
                        byte trash;
                        _tickClientsPerLayer[layer].TryRemove(tickClientToBeRemoved, out trash);
                    })
                );

            // add all new TickClients which were registered during the run
            Parallel.ForEach
                (
                _tickClientsMarkedForRegistrationPerLayer.Keys,
                layer => Parallel.ForEach(_tickClientsMarkedForRegistrationPerLayer[layer],
                    tickClientToBeRegistered => _tickClientsPerLayer[layer].TryAdd(tickClientToBeRegistered, new byte()))
                );

            // reset collections
            Parallel.ForEach
                (
                    _tickClientsPerLayer.Keys,
                    layer => {
                        _tickClientsMarkedForDeletionPerLayer[layer] = new ConcurrentBag<ITickClient>();
                        _tickClientsMarkedForRegistrationPerLayer[layer] = new ConcurrentBag<ITickClient>();
                    }
                );

            stopWatch.Stop();
            _isRunning = false;
            return stopWatch.ElapsedMilliseconds;
        }

        #endregion
    }
}