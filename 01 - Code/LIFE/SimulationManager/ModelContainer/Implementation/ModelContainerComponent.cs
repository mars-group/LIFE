// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 04.12.2014
//  *******************************************************/

using System;
using System.Collections.Generic;
using LayerAPI.Config;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using ModelContainer.Interfaces;
using SimulationManagerShared;
using SMConnector.TransportTypes;

namespace ModelContainer.Implementation {
    public class ModelContainerComponent : IModelContainer {
        private readonly ModelManagementUseCase _modelContainerUseCase;
        private readonly ModelInstantiationOrderingUseCase _modelInstantionOrderingUseCase;

        public ModelContainerComponent(SimulationManagerSettings settings) {
            _modelContainerUseCase = new ModelManagementUseCase(settings);
            _modelInstantionOrderingUseCase = new ModelInstantiationOrderingUseCase(settings);
        }

        #region IModelContainer Members

        public void RegisterForModelListChange(Action callback) {
            _modelContainerUseCase.RegisterForModelListChange(callback);
        }

        public ICollection<TModelDescription> GetAllModels() {
            return _modelContainerUseCase.GetAllModels();
        }

        public ModelContent GetModel(TModelDescription modelID) {
            return _modelContainerUseCase.GetModel(modelID);
        }

        public ModelConfig GetModelConfig(TModelDescription modelId) {
            return _modelContainerUseCase.GetModelConfig(modelId);
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

        public void AddModelFromURL(string sourceUrl) {
            _modelContainerUseCase.AddModelFromURL(sourceUrl);
        }

        #endregion
    }
}