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
using NodeRegistry.Interface;
using RTEManager.Interfaces;
using LifeAPI;
using System;
using LifeAPI.Results;
using ResultAdapter.Interface;

[assembly: InternalsVisibleTo("RTEManagerBlackBoxTest")]

namespace RTEManager.Implementation {

	internal class RTEManagerUseCase : IRTEManager {
        private readonly IResultAdapter _resultAdapter;

        // the tickClients being executed per Layer and execution group
        private readonly ConcurrentDictionary<ILayer, ConcurrentDictionary<int, ConcurrentDictionary<ITickClient, byte>>> _tickClientsPerLayer;

        // the tickClients which are marked to be deregistered per Layer
        private readonly ConcurrentDictionary<ILayer, ConcurrentDictionary<int, ConcurrentBag<ITickClient>>> _tickClientsMarkedForDeletionPerLayer;

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

        // indicates whether explicit garbage collection should be done. 
        // Only really useful for Mono execution environments
        private bool _explicitGC = false;

        // current Tick
        private int _currentTick;

		private Guid _simulationId;


        public RTEManagerUseCase(IResultAdapter resultAdapter, INodeRegistry nodeRegistry) {
            _resultAdapter = resultAdapter;
            _tickClientsPerLayer = new ConcurrentDictionary<ILayer, ConcurrentDictionary<int, ConcurrentDictionary<ITickClient, byte>>>();
            _preAndPostTickLayer = new ConcurrentBag<ISteppedActiveLayer>();
			_disposableLayers = new ConcurrentBag<IDisposableLayer> ();
            _tickClientsMarkedForDeletionPerLayer = new ConcurrentDictionary<ILayer, ConcurrentDictionary<int, ConcurrentBag<ITickClient>>>();
            _tickClientsMarkedForRegistrationPerLayer = new Dictionary<ILayer, ConcurrentBag<ITickClient>>();
            _layers = new Dictionary<TLayerInstanceId, ILayer>();
            _isRunning = false;
            _currentTick = 0;
        }

        #region Public Methods

        public void RegisterLayer(TLayerInstanceId layerInstanceId, ILayer layer) {
            if (!_tickClientsPerLayer.ContainsKey(layer)) {
                _tickClientsPerLayer.TryAdd(layer, new ConcurrentDictionary<int, ConcurrentDictionary<ITickClient, byte>>());
                _tickClientsMarkedForDeletionPerLayer.TryAdd(layer, new ConcurrentDictionary<int, ConcurrentBag<ITickClient>>());
                _tickClientsMarkedForRegistrationPerLayer.Add(layer, new ConcurrentBag<ITickClient>());
            }
            if (!_layers.ContainsKey(layerInstanceId)) {
                _layers.Add(layerInstanceId, layer);
            }

            // add layer to tickClientsPerLayer if it is an active layer
            var tickedLayer = layer as ITickClient;
            if (tickedLayer != null) {
                _tickClientsPerLayer[layer].GetOrAdd(1, new ConcurrentDictionary<ITickClient, byte>());
                _tickClientsPerLayer[layer][1].TryAdd(tickedLayer, new byte());
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
            ConcurrentDictionary<int, ConcurrentDictionary<ITickClient, byte>> bla;
            _tickClientsPerLayer.TryRemove(_layers[layerInstanceId], out bla);
            _layers.Remove(layerInstanceId);
        }

        public void UnregisterTickClient(ILayer layer, ITickClient tickClient, int executionInterval=1) {
            if (_tickClientsMarkedForDeletionPerLayer.ContainsKey(layer)) {
                _tickClientsMarkedForDeletionPerLayer[layer].GetOrAdd(executionInterval, new ConcurrentBag<ITickClient>());
                _tickClientsMarkedForDeletionPerLayer[layer][executionInterval].Add(tickClient);
            }
        }

        public void RegisterTickClient(ILayer layer, ITickClient tickClient, int executionInterval= 1) {
            if (!_isRunning) {
                // make sure execution group is available
                _tickClientsPerLayer[layer].GetOrAdd(executionInterval, new ConcurrentDictionary<ITickClient, byte>());
                // add tickClient to execution group
                _tickClientsPerLayer[layer][executionInterval].TryAdd(tickClient, new byte());
				// add tickClient to visualization if type is appropriate
				var visAgent = tickClient as ISimResult;
				if(visAgent != null){
					_resultAdapter.Register(visAgent);
                    // TODO add execution group to resultAdapter

				}
            }
            else {
                _tickClientsMarkedForRegistrationPerLayer[layer].Add(tickClient);
            }
        }

        public bool InitializeLayer(TLayerInstanceId instanceId, TInitData initData) {
			if (_simulationId == Guid.Empty) {
				// store simulationID
				_simulationId = initData.SimulationId;
				// deliver simulationID to VisulizationAdapter
				_resultAdapter.SimulationId = _simulationId;
			}
			// Initialize Layer
			var duration = _layers[instanceId].InitLayer(initData, RegisterTickClient, UnregisterTickClient);
			return duration;
        }

		public void DisposeSuitableLayers ()
		{
			Parallel.ForEach (_disposableLayers, dl => dl.DisposeLayer ());
		}

        public IEnumerable<ITickClient> GetAllTickClientsByLayer(TLayerInstanceId layer) {
            var result = new List<ITickClient>();
            foreach(var tickDict in _tickClientsPerLayer[_layers[layer]].Values){
                result.AddRange(tickDict.Keys);
            };
            return result;
        }

        public long AdvanceOneTick() {
            _isRunning = true;

            var stopWatch = Stopwatch.StartNew();

            // set currentTick to all layers
            Parallel.ForEach(_layers, l => l.Value.SetCurrentTick(_currentTick));

            // visualize all visualizable layers once prior to first execution if tick = 0
            if (_currentTick == 0) {
				Console.WriteLine ("[LIFE] Executing Pre-Viz.");
				_resultAdapter.WriteResults(_currentTick);

                if (_explicitGC) { GC.Collect(); }

                // raise tick to 1 after initial result output
                _currentTick++;
            }

			Console.WriteLine ("[LIFE] Executing Pre-Tick");
            // PreTick all ActiveLayers
            Parallel.ForEach(_preAndPostTickLayer, activeLayer => activeLayer.PreTick());
            Console.WriteLine ($"[LIFE] Executing Tick {_currentTick}...");
            // tick all tickClients
            Parallel.ForEach
                (
                    _tickClientsPerLayer.Keys,
                        layer =>
                        {
                            Parallel.ForEach(_tickClientsPerLayer[layer].Keys,
                                             executionGroup =>
                                            {
                                                // execute group's agents if they match the currenttick
                                                if (executionGroup % _currentTick == 0)
                                                {
                                                    Parallel.ForEach(_tickClientsPerLayer[layer][1],
                                                                    client => client.Key.Tick()
                                                                );
                                                }

                                            }
                                            );
                        

                            
                        }
                );
            
			Console.WriteLine ("[LIFE] Executing Post-Tick");
            // PostTick all ActiveLayers
            Parallel.ForEach(_preAndPostTickLayer, activeLayer => activeLayer.PostTick());

            // increase Tick counter
            _currentTick++;

			Console.WriteLine ("[LIFE] Executing Result Writing");
            // visualize all layers
            _resultAdapter.WriteResults(_currentTick);

			Console.WriteLine ("[LIFE] Removing agents");
            // clean up all deleted tickClients
            Parallel.ForEach
                (
                    _tickClientsMarkedForDeletionPerLayer.Keys,
                    layer => Parallel.ForEach
                        (
                            _tickClientsMarkedForDeletionPerLayer[layer],
                            tickClientsPerExecGroup => {
                                Parallel.ForEach(tickClientsPerExecGroup.Value, tickClient => {
                                     byte trash;
                                     _tickClientsPerLayer[layer][tickClientsPerExecGroup.Key].TryRemove(tickClient, out trash);

                                     // remove tickClient from visualization if type is appropiate
                                     var visAgent = tickClient as ISimResult;
                                     if (visAgent != null)
                                     {
                                         _resultAdapter.DeRegister(visAgent);
                                     }
                                 }
						    }
						)
                );

			Console.WriteLine ("[LIFE] Adding new Agents");
            // add all new TickClients which were registered during the run
            Parallel.ForEach
                (
                    _tickClientsMarkedForRegistrationPerLayer.Keys,
                    layer => Parallel.ForEach
                        (
                            _tickClientsMarkedForRegistrationPerLayer[layer],
							tickClientToBeRegistered => {								
								_tickClientsPerLayer[layer].TryAdd(tickClientToBeRegistered, new byte());

								// add tickClient to visualization if type is appropiate
								var visAgent = tickClientToBeRegistered as ISimResult;
								if(visAgent != null){
									_resultAdapter.Register(visAgent);
								}
							}
                        )
                );

			Console.WriteLine ("[LIFE] Cleaning up");
            // reset collections
            Parallel.ForEach
                (
                    _tickClientsPerLayer.Keys,
                    layer => {
						_tickClientsMarkedForDeletionPerLayer[layer] = null;
						_tickClientsMarkedForRegistrationPerLayer[layer] = null;
                        _tickClientsMarkedForDeletionPerLayer[layer] = new ConcurrentDictionary<int, ConcurrentBag<ITickClient>>();
                        _tickClientsMarkedForRegistrationPerLayer[layer] = new ConcurrentBag<ITickClient>();
                    }
                );

            // Garbage Collect
            if (_explicitGC) { GC.Collect(); }


            // stop time measurement
            stopWatch.Stop();

            _isRunning = false;
			Console.WriteLine ("[LIFE] Tick Done");
            return stopWatch.ElapsedMilliseconds;
        }

        #endregion
    }

}