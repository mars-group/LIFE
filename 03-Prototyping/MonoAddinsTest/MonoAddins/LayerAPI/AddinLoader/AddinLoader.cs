using System;
using LayerAPI.Interfaces;
using Mono.Addins;

[assembly:AddinRoot("LayerContainer", "0.1")]
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

        public TypeExtensionNode LoadLayer(Type layerType)
        {
            var res = new TypeExtensionNode();
            foreach (TypeExtensionNode node in AddinManager.GetExtensionNodes(typeof(ISteppedLayer)))
            {
                Console.WriteLine(node.Type);
                if (node.Type == layerType)
                {
                    res = node;   
                }
            }
            return res;
        }
   }
}
