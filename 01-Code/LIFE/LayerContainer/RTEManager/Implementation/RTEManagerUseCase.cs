//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LCConnector.TransportTypes;
using LifeAPI.Agent;
using LifeAPI.Layer;
using LifeAPI.Layer.Visualization;
using NodeRegistry.Interface;
using RTEManager.Interfaces;
using VisualizationAdapter.Interface;
using LifeAPI;

[assembly: InternalsVisibleTo("RTEManagerBlackBoxTest")]

namespace RTEManager.Implementation {

	internal class RTEManagerUseCase : IRTEManager {
        private readonly IVisualizationAdapterInternal _visualizationAdapter;

        // the tickClients being executed per Layer
        private readonly IDictionary<ILayer, ConcurrentDictionary<ITickClient, byte>> _tickClientsPerLayer;

        // the tickClients which are marked to be deregistered per Layer
        private readonly IDictionary<ILayer, ConcurrentBag<ITickClient>> _tickClientsMarkedForDeletionPerLayer;

        // the tickClients which are marked to be registered per Layer during active simulation
        private readonly IDictionary<ILayer, ConcurrentBag<ITickClient>> _tickClientsMarkedForRegistrationPerLayer;

        // all the layers mapped by their instanceID
        private readonly IDictionary<TLayerInstanceId, ILayer> _layers;

        // all layers which are enlisted for Pre- and PostTick calls
        private readonly ConcurrentBag<ISteppedActiveLayer> _preAndPostTickLayer;

		// all layers which are enlisted for Pre- and PostTick calls
		private readonly ConcurrentBag<IDisposableLayer> _disposableLayers;

        // indicator whether this Layercontainer ist currently executing a Tick
        private bool _isRunning;

        // current Tick
        private int _currentTick;


        public RTEManagerUseCase(IVisualizationAdapterInternal visualizationAdapter, INodeRegistry nodeRegistry) {
            _visualizationAdapter = visualizationAdapter;
            _tickClientsPerLayer = new Dictionary<ILayer, ConcurrentDictionary<ITickClient, byte>>();
            _preAndPostTickLayer = new ConcurrentBag<ISteppedActiveLayer>();
			_disposableLayers = new ConcurrentBag<IDisposableLayer> ();
            _tickClientsMarkedForDeletionPerLayer = new Dictionary<ILayer, ConcurrentBag<ITickClient>>();
            _tickClientsMarkedForRegistrationPerLayer = new Dictionary<ILayer, ConcurrentBag<ITickClient>>();
            _layers = new Dictionary<TLayerInstanceId, ILayer>();
            _isRunning = false;
            _currentTick = 0;
        }

        #region Public Methods

        public void RegisterLayer(TLayerInstanceId layerInstanceId, ILayer layer) {
            if (!_tickClientsPerLayer.ContainsKey(layer)) {
                _tickClientsPerLayer.Add(layer, new ConcurrentDictionary<ITickClient, byte>());
                _tickClientsMarkedForDeletionPerLayer.Add(layer, new ConcurrentBag<ITickClient>());
                _tickClientsMarkedForRegistrationPerLayer.Add(layer, new ConcurrentBag<ITickClient>());
            }
            if (!_layers.ContainsKey(layerInstanceId)) {
                _layers.Add(layerInstanceId, layer);
            }

            // check layer for visualizability and if true register it with the adapter
            var visualizableLayer = layer as IVisualizable;
            if (visualizableLayer != null) {
                _visualizationAdapter.RegisterVisualizable(visualizableLayer);
            }

            // add layer to tickClientsPerLayer if it is an active layer
            var tickedLayer = layer as ITickClient;
            if (tickedLayer != null) {
                _tickClientsPerLayer[layer].TryAdd(tickedLayer, new byte());
            }

            // add layer to Pre- and PostTick execution chain if it is an ISteppedActiveLayer
            var activeLayer = layer as ISteppedActiveLayer;
            if (activeLayer != null) {
                _preAndPostTickLayer.Add(activeLayer);
            }

			// add layer to Disposable execution chain if it is an IDisposableLayer
			var disposableLayer = layer as IDisposableLayer;
			if (disposableLayer != null) {
				_disposableLayers.Add (disposableLayer);
			}
        }

        public void UnregisterLayer(TLayerInstanceId layerInstanceId) {
            _tickClientsPerLayer.Remove(_layers[layerInstanceId]);
            _layers.Remove(layerInstanceId);
        }

        public void UnregisterTickClient(ILayer layer, ITickClient tickClient) {
            if (_tickClientsMarkedForDeletionPerLayer.ContainsKey(layer)) {
                _tickClientsMarkedForDeletionPerLayer[layer].Add(tickClient);
            }
        }

        public void RegisterTickClient(ILayer layer, ITickClient tickClient) {
            if (!_isRunning) {
                _tickClientsPerLayer[layer].TryAdd(tickClient, new byte());
                var visTickClient = tickClient as IVisualizable;
                if (visTickClient != null)
                {
                    _visualizationAdapter.RegisterVisualizable(visTickClient);
                }
            }
            else {
                _tickClientsMarkedForRegistrationPerLayer[layer].Add(tickClient);
            }
        }

        public bool InitializeLayer(TLayerInstanceId instanceId, TInitData initData) {
            return _layers[instanceId].InitLayer(initData, RegisterTickClient, UnregisterTickClient);
        }

		public void DisposeSuitableLayers ()
		{
			Parallel.ForEach (_disposableLayers, dl => dl.DisposeLayer ());
		}

        public IEnumerable<ITickClient> GetAllTickClientsByLayer(TLayerInstanceId layer) {
            return _tickClientsPerLayer[_layers[layer]].Keys;
        }

        public long AdvanceOneTick() {
            _isRunning = true;

            var stopWatch = Stopwatch.StartNew();

            // set currentTick to all layers
            Parallel.ForEach(_layers, l => l.Value.SetCurrentTick(_currentTick));

            // visualize all visualizable layers once prior to first execution if tick = 0
            if (_currentTick == 0) {
                _visualizationAdapter.VisualizeTick(_currentTick);
            }

            // PreTick all ActiveLayers
            Parallel.ForEach(_preAndPostTickLayer, activeLayer => activeLayer.PreTick());

            // tick all tickClients
            Parallel.ForEach
                (
                    _tickClientsPerLayer.Keys,
                    layer => Parallel.ForEach
                        (
                            _tickClientsPerLayer[layer],
                            client => client.Key.Tick()
                        )
                );

            // PostTick all ActiveLayers
            Parallel.ForEach(_preAndPostTickLayer, activeLayer => activeLayer.PostTick());

            // increase Tick counter
            _currentTick++;


            // visualize all layers
            _visualizationAdapter.VisualizeTick(_currentTick);

            // clean up all deleted tickClients
            Parallel.ForEach
                (
                    _tickClientsMarkedForDeletionPerLayer.Keys,
                    layer => Parallel.ForEach
                        (
                            _tickClientsMarkedForDeletionPerLayer[layer],
                            tickClientToBeRemoved => {
                                byte trash;
                                _tickClientsPerLayer[layer].TryRemove(tickClientToBeRemoved, out trash);
                                var visTickClient = tickClientToBeRemoved as IVisualizable;
                                if (visTickClient != null)
                                {
                                    _visualizationAdapter.DeRegisterVisualizable(visTickClient);
                                }
                            })
                );


            // add all new TickClients which were registered during the run
            Parallel.ForEach
                (
                    _tickClientsMarkedForRegistrationPerLayer.Keys,
                    layer => Parallel.ForEach
                        (
                            _tickClientsMarkedForRegistrationPerLayer[layer],
                            tickClientToBeRegistered =>
                            {
                                var visTickClient = tickClientToBeRegistered as IVisualizable;
                                if (visTickClient != null)
                                {
                                    _visualizationAdapter.RegisterVisualizable(visTickClient);
                                }
                                _tickClientsPerLayer[layer].TryAdd(tickClientToBeRegistered, new byte());
                            })
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

            // stop time measurement
            stopWatch.Stop();

            _isRunning = false;
            return stopWatch.ElapsedMilliseconds;
        }

        #endregion
    }

}