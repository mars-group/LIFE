using System;
using System.IO;
using System.Linq;
using System.Threading;
using LayerAPI.Interfaces;
using LCConnector.TransportTypes.ModelStructure;
using Mono.Addins;

[assembly: AddinRoot("LayerContainer", "0.1")]
namespace LayerAPI.AddinLoader {

    public sealed class AddinLoader : IAddinLoader {
        private ExtensionNodeList _extensionNodes;

        static readonly AddinLoader _instance = new AddinLoader();

        private AddinLoader() {
            if (Directory.Exists("./layers/addins/tmp")) { Directory.Delete("./layers/addins/tmp", true); }
            AddinManager.Initialize("./layers");
        }

        private AddinLoader(string configPath) {
            AddinManager.Initialize(configPath);
        }

        private AddinLoader(string configPath, string relativeAddinPath)
        {
            AddinManager.Initialize(configPath, relativeAddinPath);
        }

        public static AddinLoader Instance { get { return _instance; } }

        public void LoadModelContent(ModelContent modelContent) {
            // shutdown AddinManager to make sure it is not sitting on top of the files
            try {
                if (AddinManager.IsInitialized) {
                    AddinManager.Shutdown();
                }
            }
            catch (InvalidOperationException ex) {
                // ignore this case. It's stupid...
            }
            
            //write files
            modelContent.Write("./layers/addins/tmp");

            // reinitialize AddinManager
            AddinManager.Initialize("./layers");

            WaitForAddinManagerToBeInitialized();
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
            UpdateAddinRegistry();
            return AddinManager.GetExtensionNodes(typeof(ISteppedLayer));
        }

        public ExtensionNodeList LoadAllLayers(string modelName)
        {
            if (AddinManager.IsInitialized) { AddinManager.Shutdown(); }
            AddinManager.Initialize("./layers", "./addins/" + modelName);
            WaitForAddinManagerToBeInitialized();
            UpdateAddinRegistry();
            return AddinManager.GetExtensionNodes(typeof(ISteppedLayer));
        }

        private void UpdateAddinRegistry()
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
    }
}