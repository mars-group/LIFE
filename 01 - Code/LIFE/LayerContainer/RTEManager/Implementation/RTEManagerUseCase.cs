using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LayerAPI.Interfaces;
using LCConnector.TransportTypes;
using RTEManager.Interfaces;

namespace RTEManager.Implementation {
    internal class RTEManagerUseCase : IRTEManager {
        private readonly IDictionary<ILayer, ICollection<ITickClient>> tickClientsPerLayer;
        private IDictionary<ILayer, ICollection<ITickClient>> tickClientsMarkedForDeletionPerLayer;
        private readonly IDictionary<TLayerInstanceId, ILayer> layers;

        public RTEManagerUseCase() {
            tickClientsPerLayer = new Dictionary<ILayer, ICollection<ITickClient>>();
            tickClientsMarkedForDeletionPerLayer = new Dictionary<ILayer, ICollection<ITickClient>>();
            layers = new Dictionary<TLayerInstanceId, ILayer>();
        }

        #region Public Methods

        public void RegisterLayer(TLayerInstanceId layerInstanceId, ILayer layer) {
            if (!tickClientsPerLayer.ContainsKey(layer))
            {
                tickClientsPerLayer.Add(layer, new LinkedList<ITickClient>());
            }
            if (!layers.ContainsKey(layerInstanceId))
            {
                layers.Add(layerInstanceId, layer);
            }
        }

        public void UnregisterLayer(TLayerInstanceId layerInstanceId) {
            tickClientsPerLayer.Remove(layers[layerInstanceId]);
            layers.Remove(layerInstanceId);
        }

        public void UnregisterTickClient(ILayer layer, ITickClient tickClient) {
            if (tickClientsPerLayer.ContainsKey(layer)) {
                tickClientsMarkedForDeletionPerLayer[layer].Add(tickClient);
                //tickClientsPerLayer[layer].Remove(tickClient);
            }
        }

        public void RegisterTickClient(ILayer layer, ITickClient tickClient) {
            if (tickClientsPerLayer.ContainsKey(layer)) tickClientsPerLayer[layer].Add(tickClient);
        }

        public void InitializeLayer(TLayerInstanceId instanceId, TInitData initData) {
            layers[instanceId].InitLayer<TInitData>(initData, RegisterTickClient, UnregisterTickClient);
        }

        public IEnumerable<ITickClient> GetAllTickClientsByLayer(TLayerInstanceId layer) {
            return tickClientsPerLayer[layers[layer]];
        }

        public long AdvanceOneTick() {
            var now = DateTime.Now;

            Parallel.ForEach(
                tickClientsPerLayer.Keys,
                layer => Parallel.ForEach(tickClientsPerLayer[layer],
                    client => client.Tick()
                    )
                );

            var then = DateTime.Now;

            // clean up all deleted tickClients
            Parallel.ForEach
                (
                tickClientsMarkedForDeletionPerLayer.Keys,
                layer => Parallel.ForEach(tickClientsMarkedForDeletionPerLayer[layer],
                    tickClientToBeRemoved => tickClientsPerLayer[layer].Remove(tickClientToBeRemoved))
                );

            return then.Millisecond - now.Millisecond;
        }

        #endregion
    }
}