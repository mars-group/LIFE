using System.Collections.Generic;
using LayerAPI.Interfaces;

namespace RTEManager.Interfaces
{
    using LayerAPI.DataTypes;

    public interface IRTEManager
    {
        void RegisterLayer(ILayer layer, LayerInitData layerInitData);

        void UnregisterLayer(ILayer layer);

        void UnregisterTickClient(ILayer layer, ITickClient tickClient);

        void RegisterTickClient(ILayer layer, ITickClient tickClient);

        void InitializeAllLayers();

        IEnumerable<ITickClient> GetAllTickClientsByLayer(ILayer layer);

        int AdvanceOneTick();

        // TODO: Information methods needed!
    }
}
