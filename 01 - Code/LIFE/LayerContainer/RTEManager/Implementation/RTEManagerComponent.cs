namespace RTEManager.Implementation
{
    using System.Collections.Generic;

    using LayerAPI.DataTypes;
    using LayerAPI.Interfaces;

    using Interfaces;

    public class RTEManagerComponent : IRTEManager
    {
        private readonly IRTEManager _rteManagerUseCase;

        public RTEManagerComponent()
        {
            this._rteManagerUseCase = new RTEManagerUseCase();
        }


        public void RegisterLayer(ILayer layer, LayerInitData layerInitData)
        {
            _rteManagerUseCase.RegisterLayer(layer, layerInitData);
        }

        public void UnregisterLayer(ILayer layer)
        {
            _rteManagerUseCase.UnregisterLayer(layer);
        }

        public void UnregisterTickClient(ILayer layer, ITickClient tickClient)
        {
            _rteManagerUseCase.UnregisterTickClient(layer, tickClient);
        }

        public void RegisterTickClient(ILayer layer, ITickClient tickClient)
        {
            _rteManagerUseCase.RegisterTickClient(layer, tickClient);
        }

        public void InitializeAllLayers()
        {
            _rteManagerUseCase.InitializeAllLayers();
        }

        public IEnumerable<ITickClient> GetAllTickClientsByLayer(ILayer layer)
        {
            return _rteManagerUseCase.GetAllTickClientsByLayer(layer);
        }

        public int AdvanceOneTick()
        {
            return _rteManagerUseCase.AdvanceOneTick();
        }
    }
}
