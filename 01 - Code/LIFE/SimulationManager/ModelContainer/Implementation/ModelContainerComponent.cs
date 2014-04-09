using System;
using System.Collections.Generic;
using CommonTypes.TransportTypes;
using ConfigurationAdapter.Interface;
using LCConnector.TransportTypes.ModelStructure;
using ModelContainer.Interfaces;
using Shared;
using SMConnector.TransportTypes;

namespace ModelContainer.Implementation {
    public class ModelContainerComponent : IModelContainer {
        private readonly IModelContainer _modelContainerUseCase;

        public ModelContainerComponent(Configuration<SimulationManagerSettings> settings) {
            _modelContainerUseCase = new ModelContainerUseCase(settings);
        }

        public void RegisterForModelListChange(Action callback) {
            _modelContainerUseCase.RegisterForModelListChange(callback);
        }

        public ICollection<TModelDescription> GetAllModels() {
            return _modelContainerUseCase.GetAllModels();
        }

        public ModelContent GetModel(TModelDescription modelID) {
            return _modelContainerUseCase.GetModel(modelID);
        }

        public TModelDescription AddModelFromDirectory(string filePath) {
            return _modelContainerUseCase.AddModelFromDirectory(filePath);
        }

        public void DeleteModel(TModelDescription model) {
            _modelContainerUseCase.DeleteModel(model);
        }
    }
}