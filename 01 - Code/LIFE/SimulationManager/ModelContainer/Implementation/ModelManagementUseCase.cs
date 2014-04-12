using System;
using System.Collections.Generic;
using System.IO;

using LCConnector.TransportTypes.ModelStructure;
using log4net;
using Shared;
using SMConnector.TransportTypes;

namespace ModelContainer.Implementation {
    /// <summary>
    /// This class implements all logic revolving around the immediate finding of models in the model folder,<br/>
    /// watching for changes, serialization for transport and so on.
    /// </summary>
    internal class ModelManagementUseCase {
        private readonly SimulationManagerSettings _settings;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ModelManagementUseCase));
        private IDictionary<TModelDescription, string> _models;
        private FileSystemWatcher _systemWatcher;
        private ICollection<Action> _listeners;

        public ModelManagementUseCase(SimulationManagerSettings settings) {
            _settings = settings;
            _models = new Dictionary<TModelDescription, string>();
            _listeners = new LinkedList<Action>();
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

        public ICollection<TModelDescription> GetAllModels()
        {
            return _models.Keys;
        }

        public ModelContent GetModel(TModelDescription modelID) {
            if (_models.ContainsKey(modelID)) {
                return new ModelContent(_models[modelID]);
            }
            
            return null;
        }

        public TModelDescription AddModelFromDirectory(string filePath) {
            ModelContent content = new ModelContent(filePath);
            var tmp = filePath.Split(Path.DirectorySeparatorChar);
            content.Write(_settings.ModelDirectoryPath + Path.DirectorySeparatorChar + tmp[tmp.Length-1]);
            return new TModelDescription(tmp[tmp.Length - 1]);
        }

        public void DeleteModel(TModelDescription model) {
            Logger.Debug("DeleModel called");
            string path = _settings.ModelDirectoryPath + Path.DirectorySeparatorChar + model.Name;
            if (!Directory.Exists(path)) {
                Directory.Delete(path, true);
            }
            _models.Remove(model);

            Logger.Debug("Deleting model directory '" + path + "' succeeded.");
        }

        /// <summary>
        /// Recreates the _models list according to the contents in the specified model folder.
        /// </summary>
        private void UpdateModelList(object sender, FileSystemEventArgs fileSystemEventArgs) {
            Logger.Debug("Model folder '" + _settings.ModelDirectoryPath + "' was altered. Reimporting models.");

            //remove old data
            _models.Clear();

            // search through all folders in the model directory and try loading the models.
            string[] folders = Directory.GetDirectories(_settings.ModelDirectoryPath);
            foreach (var folder in folders)
            {
                string[] path = folder.Split(Path.DirectorySeparatorChar);
                try
                {
                    _models.Add(new TModelDescription(path[path.Length - 1]), folder);
                }
                catch (Exception exception)
                {
                    Logger.Error(string.Format("An error occurred while reading the model in '{0}'. Error: \n{1}", folder, exception));
                }
            }

            Logger.Debug("Finished reimporting models. Informing lsiteners");
            foreach (var listener in _listeners) {
                listener();
            }
        }
    }
}