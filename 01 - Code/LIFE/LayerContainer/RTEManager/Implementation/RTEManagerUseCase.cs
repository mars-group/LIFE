namespace RTEManager.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LayerAPI.DataTypes;
    using LayerAPI.Interfaces;

    using RTEManager.Interfaces;

    internal class RTEManagerUseCase : IRTEManager
    {

        private readonly IDictionary<ILayer, ICollection<ITickClient>> tickClientsPerLayer;

        private readonly IDictionary<ILayer, LayerInitData> initDataPerLayer; 

        public RTEManagerUseCase()
        {
            this.initDataPerLayer = new Dictionary<ILayer, LayerInitData>();
            this.tickClientsPerLayer = new Dictionary<ILayer, ICollection<ITickClient>>();
        }

        #region Public Methods

        public void RegisterLayer(ILayer layer, LayerInitData layerInitData)
        {
            if (!this.tickClientsPerLayer.ContainsKey(layer))
            {
                this.tickClientsPerLayer.Add(layer, new LinkedList<ITickClient>());
                this.initDataPerLayer.Add(layer, layerInitData);
            }
        }

        public void UnregisterLayer(ILayer layer)
        {
            this.tickClientsPerLayer.Remove(layer);
        }

        public void UnregisterTickClient(ILayer layer, ITickClient tickClient)
        {
            if (this.tickClientsPerLayer.ContainsKey(layer))
            {
                this.tickClientsPerLayer[layer].Remove(tickClient);
            }
        }

        public void RegisterTickClient(ILayer layer, ITickClient tickClient)
        {
            if (this.tickClientsPerLayer.ContainsKey(layer))
            {
                this.tickClientsPerLayer[layer].Add(tickClient);
            }
        }

        public void InitializeAllLayers()
        {
            Parallel.ForEach(this.tickClientsPerLayer.Keys, layer => layer.InitLayer(this.initDataPerLayer[layer], this.RegisterTickClient, this.UnregisterTickClient));
        }

        public IEnumerable<ITickClient> GetAllTickClientsByLayer(ILayer layer)
        {
            return this.tickClientsPerLayer[layer];
        }

        public int AdvanceOneTick()
        {
            var now = DateTime.Now;

            Parallel.ForEach(
                this.tickClientsPerLayer.Keys,
                layer => Parallel.ForEach(this.tickClientsPerLayer[layer],
                    client => client.tick()
                )
            );

            var then = DateTime.Now;
            return then.Millisecond - now.Millisecond;
        }

        #endregion
    }
}
