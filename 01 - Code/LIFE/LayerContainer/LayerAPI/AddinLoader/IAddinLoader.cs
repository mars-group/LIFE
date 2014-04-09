using System;
using LCConnector.TransportTypes.ModelStructure;
using Mono.Addins;

namespace LayerAPI.AddinLoader {
    public interface IAddinLoader {
        void UpdateAddinRegistry();

        void LoadModelContent(ModelContent modelContent);

        TypeExtensionNode LoadLayer(string layerName);
    }
}