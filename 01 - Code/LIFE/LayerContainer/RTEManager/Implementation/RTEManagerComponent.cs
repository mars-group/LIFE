namespace RTEManager.Implementation
{
    using System.Collections.Generic;

    using LayerAPI.DataTypes;
    using LayerAPI.Interfaces;

    using RTEManager.Interfaces;

    public class RTEManagerComponent : IRTEManager
    {
        private readonly RTEManagerUseCase _rteManagerUseCase;

        public RTEManagerComponent()
        {
            this._rteManagerUseCase = new RTEManagerUseCase();
        }


        public void RegisterLayer(ILayer layer, LayerInitData layerInitData)
        {
            this._rteManagerUseCase.RegisterLayer(layer, layerInitData);
        }

        public void UnregisterLayer(ILayer layer)
        {
            this._rteManagerUseCase.UnregisterLayer(layer);
        }

        public void UnregisterTickClient(ILayer layer, ITickClient tickClient)
        {
            this._rteManagerUseCase.UnregisterTickClient(layer, tickClient);
        }

        public void RegisterTickClient(ILayer layer, ITickClient tickClient)
        {
            this._rteManagerUseCase.RegisterTickClient(layer, tickClient);
        }

        public void InitializeAllLayers()
        {
            this._rteManagerUseCase.InitializeAllLayers();
        }

        public IEnumerable<ITickClient> GetAllTickClientsByLayer(ILayer layer)
        {
            return this._rteManagerUseCase.GetAllTickClientsByLayer(layer);
        }

        public int AdvanceOneTick()
        {
            return this._rteManagerUseCase.AdvanceOneTick();
        }
    }
}
