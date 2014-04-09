using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LayerAPI.DataTypes;
using LayerAPI.Interfaces;
using LCConnector.TransportTypes;
using RTEManager.Interfaces;

namespace RTEManager.Implementation {
    internal class RTEManagerUseCase : IRTEManager {
        private readonly IDictionary<TLayerInstanceId, ICollection<ITickClient>> tickClientsPerLayer;
        private readonly IDictionary<TLayerInstanceId, ILayer> layers;

        private readonly IDictionary<ILayer, LayerInitData> initDataPerLayer;

        public RTEManagerUseCase() {
            initDataPerLayer = new Dictionary<ILayer, LayerInitData>();
            tickClientsPerLayer = new Dictionary<TLayerInstanceId, ICollection<ITickClient>>();
            layers = new Dictionary<TLayerInstanceId, ILayer>();
        }

        #region Public Methods

        public void RegisterLayer(TLayerInstanceId layerInstanceId, ILayer layer) {
            if (!tickClientsPerLayer.ContainsKey(layerInstanceId))
            {
                tickClientsPerLayer.Add(layerInstanceId, new LinkedList<ITickClient>());
            }
            if (!layers.ContainsKey(layerInstanceId))
            {
                layers.Add(layerInstanceId, layer);
            }
        }

        public void UnregisterLayer(TLayerInstanceId layerInstanceId) {
            tickClientsPerLayer.Remove(layerInstanceId);
            layers.Remove(layerInstanceId);
        }

        public void UnregisterTickClient(TLayerInstanceId layerInstanceId, ITickClient tickClient) {
            if (tickClientsPerLayer.ContainsKey(layerInstanceId)) tickClientsPerLayer[layerInstanceId].Remove(tickClient);
        }

        public void RegisterTickClient(TLayerInstanceId layerInstanceId, ITickClient tickClient) {
            if (tickClientsPerLayer.ContainsKey(layerInstanceId)) tickClientsPerLayer[layerInstanceId].Add(tickClient);
        }

        public void InitializeLayer(TLayerInstanceId instanceId, TInitData initData) {
            
        }

        public IEnumerable<ITickClient> GetAllTickClientsByLayer(TLayerInstanceId layer) {
            return tickClientsPerLayer[layer];
        }

        public long AdvanceOneTick() {
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