using System;
using System.Linq;
using LayerAPI.AddinLoader;
using LayerAPI.Interfaces;

namespace LayerContainer
{

    class Program
    {
        static void Main(string[] args)
        {
            var addinLoader = new AddinLoader();
            addinLoader.LoadAddins();
            var addin = addinLoader.GetAllLayers().First<ILayer>();
            Console.WriteLine("Layer has ID: " + addin.GetID());
            Console.ReadLine();
        }
    }
}
