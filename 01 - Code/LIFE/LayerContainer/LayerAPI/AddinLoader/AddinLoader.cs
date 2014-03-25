using System;
using System.Collections.Generic;
using LayerAPI.Interfaces;
using Mono.Addins;

[assembly:AddinRoot("LayerContainer", "0.1")]
namespace LayerAPI.AddinLoader
{
    public class AddinLoader : IAddinLoader
    {
        private Dictionary<Type,ILayer> _layers;

        public AddinLoader()
        {
            _layers = new Dictionary<Type, ILayer>();
        }

        public void LoadAddins()
        {
            AddinManager.Initialize("./addinRegistry");
            AddinManager.Registry.Update();



            foreach (var layer in AddinManager.GetExtensionObjects<IEventDrivenLayer>())
            {
                _layers.Add(typeof(IEventDrivenLayer), layer);
            }

            foreach (var layer in AddinManager.GetExtensionObjects<ISteppedLayer>())
            {
                _layers.Add(typeof(IEventDrivenLayer), layer);
            }
        }

        public IEnumerable<ILayer> GetAllLayers()
        {
            return new List<ILayer>(_layers.Values);
        }


    }
}
