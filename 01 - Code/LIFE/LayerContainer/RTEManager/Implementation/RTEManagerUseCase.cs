using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LayerAPI.DataTypes;
using LayerAPI.Interfaces;
using RTEManager.Interfaces;

namespace RTEManager.Implementation {
    internal class RTEManagerUseCase : IRTEManager {
        private readonly IDictionary<ILayer, ICollection<ITickClient>> tickClientsPerLayer;

        private readonly IDictionary<ILayer, LayerInitData> initDataPerLayer;

        public RTEManagerUseCase() {
            initDataPerLayer = new Dictionary<ILayer, LayerInitData>();
            tickClientsPerLayer = new Dictionary<ILayer, ICollection<ITickClient>>();
        }

        #region Public Methods

        public void RegisterLayer(ILayer layer, LayerInitData layerInitData) {
            if (!tickClientsPerLayer.ContainsKey(layer)) {
                tickClientsPerLayer.Add(layer, new LinkedList<ITickClient>());
                initDataPerLayer.Add(layer, layerInitData);
            }
        }

        public void UnregisterLayer(ILayer layer) {
            tickClientsPerLayer.Remove(layer);
        }

        public void UnregisterTickClient(ILayer layer, ITickClient tickClient) {
            if (tickClientsPerLayer.ContainsKey(layer)) tickClientsPerLayer[layer].Remove(tickClient);
        }

        public void RegisterTickClient(ILayer layer, ITickClient tickClient) {
            if (tickClientsPerLayer.ContainsKey(layer)) tickClientsPerLayer[layer].Add(tickClient);
        }

        public void InitializeAllLayers() {
            Parallel.ForEach(tickClientsPerLayer.Keys,
                layer => layer.InitLayer(initDataPerLayer[layer], RegisterTickClient, UnregisterTickClient));
        }

        public IEnumerable<ITickClient> GetAllTickClientsByLayer(ILayer layer) {
            return tickClientsPerLayer[layer];
        }

        public int AdvanceOneTick() {
            var now = DateTime.Now;

            Parallel.ForEach(
                tickClientsPerLayer.Keys,
                layer => Parallel.ForEach(tickClientsPerLayer[layer],
                    client => client.tick()
                    )
                );

            var then = DateTime.Now;
            return then.Millisecond - now.Millisecond;
        }

        #endregion
    }
}