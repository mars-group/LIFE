using System;
using System.Collections.Generic;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using ModelContainer.Interfaces;
using Shared;
using SMConnector.TransportTypes;

namespace ModelContainer.Implementation {
    public class ModelContainerComponent : IModelContainer {
        private readonly ModelManagementUseCase _modelContainerUseCase;
        private readonly ModelInstantionOrderingUseCase _modelInstantionOrderingUseCase;

        public ModelContainerComponent(SimulationManagerSettings settings) {
            _modelContainerUseCase = new ModelManagementUseCase(settings);
            _modelInstantionOrderingUseCase = new ModelInstantionOrderingUseCase(settings);
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

        public IList<TLayerDescription> GetInstantiationOrder(TModelDescription model) {
            return _modelInstantionOrderingUseCase.GetInstantiationOrder(model);
        }
    }
}