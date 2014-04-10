using LayerAPI.Interfaces;
using LCConnector.TransportTypes.ModelStructure;
using Mono.Addins;

[assembly: AddinRoot("LayerContainer", "0.1")]

namespace LayerAPI.AddinLoader {
    public class AddinLoader : IAddinLoader {
        public AddinLoader() {
            AddinManager.Initialize("./addinRegistry");
            AddinManager.Registry.Update();
        }

        public void LoadModelContent(ModelContent modelContent) {
            modelContent.Write("./addinRegistry/addins");
            UpdateAddinRegistry();
        }

        public TypeExtensionNode LoadLayer(string layerName) {

            foreach (TypeExtensionNode node in AddinManager.GetExtensionNodes(typeof (ISteppedLayer))) {
                if (node.Type.ToString() == layerName) return node;
            }
            return null;
        }

        private void UpdateAddinRegistry()
        {
            AddinManager.Registry.Update();
        }
    }
}