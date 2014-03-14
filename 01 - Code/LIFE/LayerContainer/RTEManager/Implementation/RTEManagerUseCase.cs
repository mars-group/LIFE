using System.Collections.Generic;
using LayerAPI.Interfaces;
using RTEManager.Interfaces;

namespace RTEManager
{
    class RTEManagerUseCase : IRTEManager
    {
        public void RegisterLayer(ILayer layer)
        {
            throw new System.NotImplementedException();
        }

        public void UnregisterLayer(ILayer layer)
        {
            throw new System.NotImplementedException();
        }

        public void UnregisterTickClient(ITickClient tickClient)
        {
            throw new System.NotImplementedException();
        }

        public void RegisterTickClient(ITickClient tickClient)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ITickClient> GetAllTickClients()
        {
            throw new System.NotImplementedException();
        }

        public long AdvanceOneTick()
        {
            throw new System.NotImplementedException();
        }
    }
}
