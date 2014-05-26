using LayerAPI.Interfaces;
using LCConnector.TransportTypes.ModelStructure;
using Mono.Addins;

[assembly: AddinRoot("LayerContainer", "0.1")]

namespace LayerAPI.AddinLoader {

    public class AddinLoader : IAddinLoader {
        public AddinLoader() {
            AddinManager.Initialize("./layers");
            AddinManager.Registry.Update();
        }

        public AddinLoader(string configPath) {
            AddinManager.Initialize(configPath);
        }

        public AddinLoader(string configPath, string relativeAddinPath)
        {
            AddinManager.Initialize(configPath, relativeAddinPath);
        }

        public void LoadModelContent(ModelContent modelContent) {
            modelContent.Write("./layers/addins");
            UpdateAddinRegistry();
        }

        public TypeExtensionNode LoadLayer(string layerName) {
            this.UpdateAddinRegistry();

            foreach (TypeExtensionNode node in AddinManager.GetExtensionNodes(typeof (ISteppedLayer))) {
                if (node.Type.Name == layerName) return node;
            }
            return null;
        }

        public ExtensionNodeList LoadAllLayers() {
            this.UpdateAddinRegistry();
            return AddinManager.GetExtensionNodes(typeof(ISteppedLayer));
        }

        private void UpdateAddinRegistry()
        {
            AddinManager.Registry.Update();
        }
    }
}