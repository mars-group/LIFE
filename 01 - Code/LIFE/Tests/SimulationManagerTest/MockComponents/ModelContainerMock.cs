using System;
using System.Collections.Generic;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using ModelContainer.Interfaces;
using SMConnector.TransportTypes;

namespace SimulationManagerTest.MockComponents {
    internal class ModelContainerMock : IModelContainer {
        public void RegisterForModelListChange(Action callback) {
            throw new NotImplementedException("Don't mock me, I'm only a mock :(");
        }

        public ICollection<TModelDescription> GetAllModels() {
            return new List<TModelDescription> {new TModelDescription("TestSimulationModel")};
        }

        public TModelDescription AddModelFromDirectory(string filePath) {
            throw new NotImplementedException("Don't mock me, I'm only a mock :(");
        }

        public IList<TLayerDescription> GetInstantiationOrder(TModelDescription model) {
            return new[] {new TLayerDescription("TestLayer", 0, 1, "TestLayer.dll") };
        }

        public void DeleteModel(TModelDescription model) {
            throw new NotImplementedException("Don't mock me, I'm only a mock :(");
        }

        public ModelContent GetModel(TModelDescription modelID) {
            throw new NotImplementedException("Don't mock me, I'm only a mock :(");
        }
    }
}