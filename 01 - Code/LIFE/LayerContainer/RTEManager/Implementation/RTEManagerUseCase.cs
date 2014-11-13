using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using LayerAPI.Interfaces;
using LayerAPI.Interfaces.Visualization;
using LCConnector.TransportTypes;
using RTEManager.Interfaces;
using VisualizationAdapter.Interface;

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
        private readonly List<ISteppedActiveLayer> _preAndPostTickLayer;

        // indicator whether this Layercontainer ist currently executing a Tick
        private bool _isRunning;

        // current Tick
        private int _currentTick;


        public RTEManagerUseCase(IVisualizationAdapterInternal visualizationAdapter) {
            _visualizationAdapter = visualizationAdapter;

            _tickClientsPerLayer = new Dictionary<ILayer, ConcurrentDictionary<ITickClient, byte>>();
            _preAndPostTickLayer = new List<ISteppedActiveLayer>();
            _tickClientsMarkedForDeletionPerLayer = new Dictionary<ILayer, ConcurrentBag<ITickClient>>();
            _tickClientsMarkedForRegistrationPerLayer = new Dictionary<ILayer, ConcurrentBag<ITickClient>>();
            _layers = new Dictionary<TLayerInstanceId, ILayer>();
            _isRunning = false;
            _currentTick = 0;
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
            _currentTick++;
            var stopWatch = Stopwatch.StartNew();

            // PreTick all ActiveLayers
            Parallel.ForEach(_preAndPostTickLayer, activeLayer => activeLayer.PreTick());

            // tick all tickClients
            Parallel.ForEach(
                _tickClientsPerLayer.Keys,
                layer => Parallel.ForEach(_tickClientsPerLayer[layer],
                    client => client.Key.Tick()
                    )
                );

            // visualize all visualizable layers
            _visualizationAdapter.VisualizeTick(_currentTick);

            // PostTick all ActiveLayers
            Parallel.ForEach(_preAndPostTickLayer, activeLayer => activeLayer.PostTick());

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
                    layer =>
                    {
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