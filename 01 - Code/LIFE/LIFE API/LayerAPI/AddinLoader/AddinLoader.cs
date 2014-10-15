using System.IO;
using System.Linq;
using System.Threading;
using LayerAPI.Interfaces;
using LCConnector.TransportTypes.ModelStructure;
using Mono.Addins;

[assembly: AddinRoot("LayerContainer", "0.1")]

namespace LayerAPI.AddinLoader {

    public class AddinLoader : IAddinLoader {
        private ExtensionNodeList _extensionNodes;

        public AddinLoader() {
            EmptyDirectory("./layers");
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
            WaitForAddinManagerToBeInitialized();
            modelContent.Write("./layers/addins");
            UpdateAddinRegistry();
            _extensionNodes = AddinManager.GetExtensionNodes(typeof (ISteppedLayer));
        }

        public TypeExtensionNode LoadLayer(string layerName) {
            WaitForAddinManagerToBeInitialized();
            UpdateAddinRegistry();
            return _extensionNodes.Cast<TypeExtensionNode>().First(node => node.Type.Name == layerName);
        }

        public ExtensionNodeList LoadAllLayers() {
            WaitForAddinManagerToBeInitialized();
            return AddinManager.GetExtensionNodes(typeof(ISteppedLayer));
        }

        public void UpdateAddinRegistry()
        {
            WaitForAddinManagerToBeInitialized();
            AddinManager.Registry.Update();
        }

        private void WaitForAddinManagerToBeInitialized() {
            while (!AddinManager.IsInitialized)
            {
                Thread.Sleep(50);
            }
        }

        private static void EmptyDirectory(string targetDirectory)
        {

            var dirInfo = new DirectoryInfo(targetDirectory);

            foreach (var file in dirInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (var dir in dirInfo.GetDirectories())
            {
                dir.Delete(true);
            }

        }
    }
}