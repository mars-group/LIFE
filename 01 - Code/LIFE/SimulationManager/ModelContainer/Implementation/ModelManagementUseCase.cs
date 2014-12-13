// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ConfigurationAdapter.Interface;
using Ionic.Zip;
using LCConnector.TransportTypes.ModelStructure;
using LifeAPI.Config;
using log4net;
using SimulationManagerShared;
using SMConnector.TransportTypes;

namespace ModelContainer.Implementation {
    /// <summary>
    ///     This class implements all logic revolving around the immediate finding of models in the model folder,<br />
    ///     watching for changes, serialization for transport and so on.
    /// </summary>
    internal class ModelManagementUseCase {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ModelManagementUseCase));
        private readonly SimulationManagerSettings _settings;
        private IDictionary<TModelDescription, string> _models;
        private FileSystemWatcher _systemWatcher;
        private ICollection<Action> _listeners;

        public ModelManagementUseCase(SimulationManagerSettings settings) {
            _settings = settings;
            _models = new Dictionary<TModelDescription, string>();
            _listeners = new LinkedList<Action>();
            if (!Directory.Exists(_settings.ModelDirectoryPath))
                Directory.CreateDirectory(_settings.ModelDirectoryPath);
            // delete possible remains from old run
            if (Directory.Exists("./layers/addins/tmp")) Directory.Delete("./layers/addins/tmp", true);


            try {
                _systemWatcher = new FileSystemWatcher(_settings.ModelDirectoryPath);
            }
            catch {
                Directory.CreateDirectory(_settings.ModelDirectoryPath);
                _systemWatcher = new FileSystemWatcher(_settings.ModelDirectoryPath);
            }


            //Reload model folder contents if file system has changed. (Also of course once, initially)
            _systemWatcher.Changed += UpdateModelList;
            UpdateModelList(null, null);

            Logger.Debug("instantiated");
        }

        public void RegisterForModelListChange(Action callback) {
            _listeners.Add(callback);
        }

        public ICollection<TModelDescription> GetAllModels() {
            return _models.Keys;
        }

        public ModelContent GetModel(TModelDescription modelDesc) {
            if (_models.ContainsKey(modelDesc)) return new ModelContent(_models[modelDesc]);

            return null;
        }

        public TModelDescription AddModelFromDirectory(string filePath) {
            ModelContent content = new ModelContent(filePath);
            string[] tmp = filePath.Split(Path.DirectorySeparatorChar);
            content.Write(_settings.ModelDirectoryPath + Path.DirectorySeparatorChar + tmp[tmp.Length - 1]);
            return new TModelDescription(tmp[tmp.Length - 1]);
        }

        public void AddModelFromURL(string sourceUrl) {
            WebClient webClient = new WebClient();
            string targetFileName = _settings.ModelDirectoryPath + Path.DirectorySeparatorChar + "tmp.zip";
            webClient.DownloadFile(sourceUrl, targetFileName);
            ZipFile zip = new ZipFile(targetFileName);
            zip.ExtractAll(".");
        }

        public void DeleteModel(TModelDescription model) {
            Logger.Debug("DeleteModel called");
            string path = _settings.ModelDirectoryPath + Path.DirectorySeparatorChar + model.Name;
            if (!Directory.Exists(path)) Directory.Delete(path, true);
            _models.Remove(model);

            Logger.Debug("Deleting model directory '" + path + "' succeeded.");
        }

        /// <summary>
        ///     Recreates the _models list according to the contents in the specified model folder.
        /// </summary>
        private void UpdateModelList(object sender, FileSystemEventArgs fileSystemEventArgs) {
            Logger.Debug("Model folder '" + _settings.ModelDirectoryPath + "' was altered. Reimporting models.");

            //remove old data
            _models.Clear();

            // search through all folders in the model directory and try loading the models.
            string[] folders = Directory.GetDirectories(_settings.ModelDirectoryPath);
            foreach (string folder in folders) {
                string[] path = folder.Split(Path.DirectorySeparatorChar);
                try {
                    _models.Add(new TModelDescription(path[path.Length - 1]), folder);
                }
                catch (Exception exception) {
                    Logger.Error
                        (string.Format
                            ("An error occurred while reading the model in '{0}'. Error: \n{1}", folder, exception));
                }
            }

            Logger.Debug("Finished reimporting models. Informing lsiteners");
            foreach (Action listener in _listeners) {
                listener();
            }
        }

        public ModelConfig GetModelConfig(TModelDescription model) {
            string path = _settings.ModelDirectoryPath + Path.DirectorySeparatorChar + model.Name + Path.DirectorySeparatorChar + model.Name + ".cfg";
            return Configuration.Load<ModelConfig>(path);
        }
    }
}