
using System.Collections.Generic;
using LayerAPI.Interfaces;

namespace LayerAPI.AddinLoader
{
    public interface IAddinLoader
    {
        void LoadAddins();

        IEnumerable<ILayer> GetAllLayers(); 
    }
}
