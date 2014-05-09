using System;
using System.Collections;
using System.Collections.Generic;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using ModelContainer.Interfaces;
using SMConnector.TransportTypes;

namespace SimulationManagerTest.MockComponents {
    internal class ModelContainerMock : IModelContainer {
        public void RegisterForModelListChange(Action callback) {
            throw new NotImplementedException();
        }

        public ICollection<TModelDescription> GetAllModels() {
            throw new NotImplementedException();
        }

        public TModelDescription AddModelFromDirectory(string filePath) {
            throw new NotImplementedException();
        }

        public IList<TLayerDescription> GetInstantiationOrder(TModelDescription model) {
            throw new NotImplementedException();
        }

        public void DeleteModel(TModelDescription model) {
            throw new NotImplementedException();
        }

        public ModelContent GetModel(TModelDescription modelID) {
            throw new NotImplementedException();
        }
    }
}