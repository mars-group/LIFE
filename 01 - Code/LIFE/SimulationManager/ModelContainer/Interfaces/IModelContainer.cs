
using System;
using System.Collections.Generic;
using CommonTypes.TransportTypes;

namespace ModelContainer.Interfaces
{
    public interface IModelContainer
    {
        IList<TSimModel> GetAllModels();

        TSimModel GetModel(int modelID);

        void AddModel(TModel model);

        void AddModelFromFile(String filePath);

        void DeleteModel(TModel model);

        void DeleteModel(int modelID);
    }
}
