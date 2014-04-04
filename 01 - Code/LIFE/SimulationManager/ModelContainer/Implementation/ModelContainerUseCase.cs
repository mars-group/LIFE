using System;
using System.Collections.Generic;
using CommonTypes.TransportTypes;
using ModelContainer.Interfaces;

namespace ModelContainer.Implementation {
    internal class ModelContainerUseCase : IModelContainer {
        public IList<TSimModel> GetAllModels() {
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
            throw new NotImplementedException();
        }
    }
}