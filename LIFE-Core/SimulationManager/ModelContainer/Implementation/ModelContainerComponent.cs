//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System.Collections.Generic;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using LIFE.API.Config;
using ModelContainer.Interfaces;
using Newtonsoft.Json.Linq;
using SimulationManagerShared;
using SMConnector.TransportTypes;

namespace ModelContainer.Implementation
{
    public class ModelContainerComponent : IModelContainer
    {
        private readonly ModelManagementUseCase _modelContainerUseCase;
        private readonly ModelInstantiationOrderingUseCase _modelInstantionOrderingUseCase;

        public ModelContainerComponent(SimulationManagerSettings settings)
        {
            _modelContainerUseCase = new ModelManagementUseCase(settings);
            _modelInstantionOrderingUseCase = new ModelInstantiationOrderingUseCase();
        }

        #region IModelContainer Members

        public TModelDescription GetModelDescription(string modelPath)
        {
            return _modelContainerUseCase.GetModelDescription(modelPath);
        }

        public ModelContent GetSerializedModel(TModelDescription modelPath)
        {
            return _modelContainerUseCase.GetModel(modelPath);
        }

        public ModelConfig GetModelConfig(TModelDescription modelId)
        {
            return _modelContainerUseCase.GetModelConfig(modelId);
        }

        public IList<TLayerDescription> GetInstantiationOrder(TModelDescription model)
        {
            return _modelInstantionOrderingUseCase.GetInstantiationOrder(model);
        }

        public JObject GetScenarioConfig(string scenarioConfigId)
        {
            return _modelContainerUseCase.GetScenarioConfig(scenarioConfigId);
        }

        #endregion
    }
}