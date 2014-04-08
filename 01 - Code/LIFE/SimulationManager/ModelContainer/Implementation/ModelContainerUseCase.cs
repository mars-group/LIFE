using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using CommonTypes.TransportTypes;
using LCConnector.TransportTypes;
using log4net;
using ModelContainer.Interfaces;

namespace ModelContainer.Implementation {
    internal class ModelContainerUseCase : IModelContainer {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ModelContainerUseCase));
        private string _modelDirectory;
        private IDictionary<TLayerDescription, byte[]> _layers;
        private XmlSerializer _modelSerializer;

        public ModelContainerUseCase() {
            _layers = new Dictionary<TLayerDescription, byte[]>();
            _modelSerializer = new XmlSerializer(typeof(TLayerDescription));
        }

        public IList<TSimModel> GetAllModels() {
            //remove old data
            _layers.Clear();

            // search through all folders in the model directory and try loading the models.
            IList<TSimModel> models = new List<TSimModel>();
            string[] folders = Directory.GetDirectories(_modelDirectory);
            foreach (var folder in folders) {
                try {
                }
                catch (Exception exception) {
                    Logger.Error(string.Format("An error occurred while reading the model in '{0}'. Error: \n{1}", folder, exception));
                }
            }

            throw new NotImplementedException();
        }

        public TSimModel GetModel(int modelID) {
            throw new NotImplementedException();
        }

        public void AddModel(TModel model) {
            throw new NotImplementedException();
        }

        public void AddModelFromFile(string filePath) {
            throw new NotImplementedException();
        }

        public void DeleteModel(TModel model) {
            throw new NotImplementedException();
        }

        public void DeleteModel(int modelID) {
            
        }
    }
}