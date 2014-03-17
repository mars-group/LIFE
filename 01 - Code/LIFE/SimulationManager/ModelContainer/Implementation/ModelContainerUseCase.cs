using System.Collections.Generic;
using CommonTypes.TransportTypes;
using ModelContainer.Interfaces;

namespace ModelContainer.Implementation
{
    class ModelContainerUseCase : IModelContainer
    {
        public IList<TSimModel> GetAllModels()
        {
            throw new System.NotImplementedException();
        }

        public TSimModel GetModel(int modelID)
        {
            throw new System.NotImplementedException();
        }

        public void AddModel(TModel model)
        {
            throw new System.NotImplementedException();
        }

        public void AddModelFromFile(string filePath)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteModel(TModel model)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteModel(int modelID)
        {
            throw new System.NotImplementedException();
        }
    }
}
