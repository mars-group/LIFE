using System.Collections.Generic;
using LayerAPI.Interfaces;
using RTEManager.Interfaces;


namespace RTEManager
{
    public class RTEManagerComponent : IRTEManager
    {
        private readonly RTEManagerUseCase _rteManagerUseCase;

        public RTEManagerComponent()
        {
            _rteManagerUseCase = new RTEManagerUseCase();
        }


        public void RegisterLayer(ILayer layer)
        {
            _rteManagerUseCase.RegisterLayer(layer);
        }

        public void UnregisterLayer(ILayer layer)
        {
            _rteManagerUseCase.UnregisterLayer(layer);
        }

        public void UnregisterTickClient(ITickClient tickClient)
        {
            _rteManagerUseCase.UnregisterTickClient(tickClient);
        }

        public void RegisterTickClient(ITickClient tickClient)
        {
            _rteManagerUseCase.RegisterTickClient(tickClient);
        }

        public IEnumerable<ITickClient> GetAllTickClients()
        {
            return _rteManagerUseCase.GetAllTickClients();
        }

        public long AdvanceOneTick()
        {
            return _rteManagerUseCase.AdvanceOneTick();
        }
    }
}
