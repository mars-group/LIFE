using System;
using System.Linq;
using LayerAPI.AddinLoader;
using LayerAPI.Interfaces;

namespace LayerContainerFacade
{

    class Program
    {
        static void Main(string[] args)
        {
            var _facade = ApplicationCoreFactory.GetLayerContainerFacade();
        }
    }
}
