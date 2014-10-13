using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LayerAPI.Interfaces;
using LCConnector.TransportTypes.ModelStructure;
using Mono.Addins;

[assembly: AddinRoot("LayerContainer", "0.1")]

namespace LayerAPI.AddinLoader {

    public class AddinLoader : IAddinLoader {
        private ExtensionNodeList _extensionNodes;

        public AddinLoader() {
            AddinManager.Initialize("./layers");
        }

        public AddinLoader(string configPath) {
            AddinManager.Initialize(configPath);
        }

        public AddinLoader(string configPath, string relativeAddinPath)
        {
            AddinManager.Initialize(configPath, relativeAddinPath);
        }

        public void LoadModelContent(ModelContent modelContent) {
            while (!AddinManager.IsInitialized) { }
            modelContent.Write("./layers/addins");
            UpdateAddinRegistry();
            _extensionNodes = AddinManager.GetExtensionNodes(typeof (ISteppedLayer));
        }

        public TypeExtensionNode LoadLayer(string layerName) {
            //UpdateAddinRegistry();
            return _extensionNodes.Cast<TypeExtensionNode>().First(node => node.Type.Name == layerName);
        }

        public ExtensionNodeList LoadAllLayers() {
            return AddinManager.GetExtensionNodes(typeof(ISteppedLayer));
        }

        public void UpdateAddinRegistry()
        {
            AddinManager.Registry.Update();
        }
    }
}