using System;
using LayerAPI.Interfaces;
using Mono.Addins;

[assembly: AddinRoot("LayerContainer", "0.1")]
namespace LayerAPI.AddinLoader
{
    public class AddinLoader : IAddinLoader
    {

        public AddinLoader()
        {
            AddinManager.Initialize("./addinRegistry");
            AddinManager.Registry.Update();
        }

        public void UpdateAddinRegistry()
        {
            AddinManager.Registry.Update();
        }

        public TypeExtensionNode LoadLayer(Uri layerUri, Type layerType)
        {
            // TODO:
            // 1. download DLL from layerUri
            // 2. copy to ./addinRegistry folder
            UpdateAddinRegistry();

            foreach (TypeExtensionNode node in AddinManager.GetExtensionNodes(typeof(ISteppedLayer)))
            {
                if (node.Type == layerType)
                {
                    return node;
                }
            }
            return null;
        }
    }
}
