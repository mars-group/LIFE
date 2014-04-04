using System.Collections.Generic;
using CommonTypes.TransportTypes;
using ModelContainer.Interfaces;

namespace ModelContainer.Implementation {
    public class ModelContainerComponent : IModelContainer {
        private readonly IModelContainer _modelContainerUseCase;

        public ModelContainerComponent() {
            _modelContainerUseCase = new ModelContainerUseCase();
        }

        public IList<TSimModel> GetAllModels() {
            return _modelContainerUseCase.GetAllModels();
        }

        public TSimModel GetModel(int modelID) {
            return _modelContainerUseCase.GetModel(modelID);
        }

        public void AddModel(TModel model) {
            _modelContainerUseCase.AddModel(model);
        }

        public void AddModelFromFile(string filePath) {
            _modelContainerUseCase.AddModelFromFile(filePath);
        }

        public void DeleteModel(TModel model) {
            _modelContainerUseCase.DeleteModel(model);
        }

        public void DeleteModel(int modelID) {
            _modelContainerUseCase.DeleteModel(modelID);
        }
    }
}