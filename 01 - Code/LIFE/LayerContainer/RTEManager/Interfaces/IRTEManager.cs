using System.Collections.Generic;
using LayerAPI.Interfaces;

namespace RTEManager.Interfaces
{
    public interface IRTEManager
    {
        void RegisterLayer(ILayer layer);

        void UnregisterLayer(ILayer layer);

        void UnregisterTickClient(ITickClient tickClient);

        void RegisterTickClient(ITickClient tickClient);

        IEnumerable<ITickClient> GetAllTickClients();

        long AdvanceOneTick();
    }
}
