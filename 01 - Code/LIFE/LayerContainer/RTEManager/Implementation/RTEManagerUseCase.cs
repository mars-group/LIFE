using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hik.Threading;
using LayerAPI.Interfaces;
using LCConnector.TransportTypes;
using RTEManager.Interfaces;

namespace RTEManager.Implementation {
    internal class RTEManagerUseCase : IRTEManager {
        private readonly IDictionary<ILayer, ICollection<ITickClient>> _tickClientsPerLayer;
        private IDictionary<ILayer, ICollection<ITickClient>> _tickClientsMarkedForDeletionPerLayer;
        private readonly IDictionary<TLayerInstanceId, ILayer> _layers;

        public RTEManagerUseCase() {
            _tickClientsPerLayer = new Dictionary<ILayer, ICollection<ITickClient>>();
            _tickClientsMarkedForDeletionPerLayer = new Dictionary<ILayer, ICollection<ITickClient>>();
            _layers = new Dictionary<TLayerInstanceId, ILayer>();
        }

        #region Public Methods

        public void RegisterLayer(TLayerInstanceId layerInstanceId, ILayer layer) {
            if (!_tickClientsPerLayer.ContainsKey(layer))
            {
                _tickClientsPerLayer.Add(layer, new ThreadSafeList<ITickClient>());
                _tickClientsMarkedForDeletionPerLayer.Add(layer, new LinkedList<ITickClient>());
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
                //tickClientsPerLayer[layer].Remove(tickClient);
            }
        }

        public void RegisterTickClient(ILayer layer, ITickClient tickClient) {
            if (_tickClientsPerLayer.ContainsKey(layer)) _tickClientsPerLayer[layer].Add(tickClient);
        }

        public void InitializeLayer(TLayerInstanceId instanceId, TInitData initData) {
            _layers[instanceId].InitLayer<TInitData>(initData, RegisterTickClient, UnregisterTickClient);
        }

        public IEnumerable<ITickClient> GetAllTickClientsByLayer(TLayerInstanceId layer) {
            return _tickClientsPerLayer[_layers[layer]];
        }

        public long AdvanceOneTick() {
            var now = DateTime.Now;

            Parallel.ForEach(
                _tickClientsPerLayer.Keys,
                layer => Parallel.ForEach(_tickClientsPerLayer[layer],
                    client => client.Tick()
                    )
                );

            var then = DateTime.Now;

            // clean up all deleted tickClients
            Parallel.ForEach
                (
                _tickClientsMarkedForDeletionPerLayer.Keys,
                layer => Parallel.ForEach(_tickClientsMarkedForDeletionPerLayer[layer],
                    tickClientToBeRemoved => _tickClientsPerLayer[layer].Remove(tickClientToBeRemoved))
                );

            _tickClientsMarkedForDeletionPerLayer = new Dictionary<ILayer, ICollection<ITickClient>>();
            return then.Millisecond - now.Millisecond;
        }

        #endregion
    }
}