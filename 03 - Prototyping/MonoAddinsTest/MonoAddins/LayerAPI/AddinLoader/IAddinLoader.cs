using System;
using Mono.Addins;

namespace LayerAPI.AddinLoader
{
    public interface IAddinLoader
    {
        void UpdateAddinRegistry();

        TypeExtensionNode LoadLayer(Type layerType);
    }
}
