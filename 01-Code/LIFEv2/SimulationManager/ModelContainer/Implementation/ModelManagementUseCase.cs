//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using ConfigurationAdapter.Interface;
using LCConnector.TransportTypes.ModelStructure;
using LifeAPI.Config;
using MARS.Shuttle.SimulationConfig.Interfaces;
using SimulationManagerShared;
using SMConnector.TransportTypes;
using System.Linq;
using ModelContainer.Interfaces.Exceptions;

namespace ModelContainer.Implementation
{
    /// <summary>
    ///     This class implements all logic revolving around the immediate finding of models in the model folder,<br />
    ///     watching for changes, serialization for transport and so on.
    /// </summary>
    internal class ModelManagementUseCase {
        private TModelDescription _currentModel;

        public ModelManagementUseCase(SimulationManagerSettings settings) {
            FileSystemWatcher systemWatcher;
        }


        public TModelDescription GetModelDescription(string modelPath)
        {
            if (_currentModel == null)
            {
                CheckForValidModel(modelPath);
            }

            return _currentModel;
        }

        public ModelContent GetModel(TModelDescription modelDesc)
        {
            if (_currentModel == null)
            {
                CheckForValidModel(modelDesc.ModelPath);
            }
            return _currentModel.Equals(modelDesc) ? new ModelContent(_currentModel.ModelPath) : null;
        }

        private void CheckForValidModel(string path)
        {
            var addinLoader = new LayerLoader.Implementation.LayerLoader();
            var nodes = addinLoader.LoadAllLayersForModel(path);
            _currentModel = nodes.Any() ? new TModelDescription(path) : null;

            if (_currentModel == null)
            {
                throw new Exception("No Model has been loaded so far! Exiting...");
            }
        }


        public ISimConfig GetShuttleSimConfig(TModelDescription model, string simConfigName) {

            var path = $"{model.ModelPath}{Path.DirectorySeparatorChar}scenarios{Path.DirectorySeparatorChar}{simConfigName}";
            if (!File.Exists(path)) {
                throw new NoSimulationConfigFoundException("No SimConfig.json could be found! Please verify that you created one via MARS SHUTTLE and packed your image accoridngly.");
            }

            var simConfigJsonContent = File.ReadAllText(path);

            return SimConfigObjectDeserializer.GetDeserializedSimConfigObject(simConfigJsonContent);
        }

        public ModelConfig GetModelConfig(TModelDescription model)
        {
            var path = model.ModelPath + Path.DirectorySeparatorChar + model.Name + ".cfg";


            //config exists, so load it
            if (File.Exists(path))
            {
                return Configuration.Load<ModelConfig>(path);
            }

            // config does not exist, create the default one
            var addinLoader = new LayerLoader.Implementation.LayerLoader();
            var nodes = addinLoader.LoadAllLayersForModel(model.ModelPath);
            var layerConfigs = nodes.Select(node => new LayerConfig(node.LayerType.Name, DistributionStrategy.NO_DISTRIBUTION, new List<AgentConfig>())).ToList();
            var mc = new ModelConfig(layerConfigs);
            Configuration.Save(mc, path);
            return mc;
        }
    }
}