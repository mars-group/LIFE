// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System;
using System.IO;
using System.Linq;
using System.Threading;
using LCConnector.TransportTypes.ModelStructure;
using LifeAPI.Layer;
using Mono.Addins;

[assembly: AddinRoot("LayerContainer", "0.1")]

namespace LifeAPI.AddinLoader {
    public sealed class AddinLoader : IAddinLoader {
        public static AddinLoader Instance { get { return _instance; } }
        private static readonly AddinLoader _instance = new AddinLoader();
        private ExtensionNodeList _extensionNodes;

        private AddinLoader() {
            var applicationName = AppDomain.CurrentDomain.FriendlyName.Split(new[] { '.' }, 2)[0];
            if (applicationName == "LayerContainer")
            {
                if (Directory.Exists("./layers")) Directory.Delete("./layers", true);
            }
            if (Directory.Exists("./layers/addins/tmp")) Directory.Delete("./layers/addins/tmp", true);
            AddinManager.Initialize("./layers");
        }

        private AddinLoader(string configPath) {
            AddinManager.Initialize(configPath);
        }

        private AddinLoader(string configPath, string relativeAddinPath) {
            AddinManager.Initialize(configPath, relativeAddinPath);
        }

        #region IAddinLoader Members

        public void LoadModelContent(ModelContent modelContent) {
            // shutdown AddinManager to make sure it is not sitting on top of the files
            try {
                if (AddinManager.IsInitialized) AddinManager.Shutdown();
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

            return AddinManager.GetExtensionNodes(typeof (ISteppedLayer));
        }

        public ExtensionNodeList LoadAllLayers(string modelName) {
			try {
				if (AddinManager.IsInitialized) AddinManager.Shutdown();
			}
			catch (InvalidOperationException ex) {
				// ignore this case. It's stupid...
			}
            AddinManager.Initialize("./layers", "./addins/" + modelName);
            WaitForAddinManagerToBeInitialized();
            UpdateAddinRegistry();
            return AddinManager.GetExtensionNodes(typeof (ISteppedLayer));
        }

        #endregion

        private void UpdateAddinRegistry() {
            WaitForAddinManagerToBeInitialized();
            AddinManager.Registry.Update();
        }

        private void WaitForAddinManagerToBeInitialized() {
            while (!AddinManager.IsInitialized) {
                Thread.Sleep(50);
            }
        }
    }
}