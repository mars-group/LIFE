
using System;
using System.Reflection;
using LayerAPI.Interfaces;
using Mono.Addins;

namespace LayerContainer
{
    [assembly:AddinRoot ("LayerContainer","0.1")]
    class Program
    {
        static void Main(string[] args)
        {
            AddinManager.Initialize();
            AddinManager.Registry.Update();
            foreach (var eventLayer in AddinManager.GetExtensionObjects<IEventDrivenLayer>())
            {
                eventLayer.StartLayer();

            }
        }
    }
}
