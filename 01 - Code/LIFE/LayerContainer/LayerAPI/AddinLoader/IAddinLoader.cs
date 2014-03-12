
using System.Collections.Generic;
using LayerAPI.Interfaces;

namespace LayerAPI.AddinLoader
{
    interface IAddinLoader
    {
        void LoadAddins();

        IEnumerable<ILayer> GetAllLayers(); 
    }
}
