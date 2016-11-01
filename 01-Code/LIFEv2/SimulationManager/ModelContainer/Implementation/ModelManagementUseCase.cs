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
using System.Net;
using System.Net.Http;
using System.Text;
using CommonTypes;
using ConfigService;
using MarsEurekaClient;
using ModelContainer.Interfaces.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog.LayoutRenderers.Wrappers;
using static System.String;

namespace ModelContainer.Implementation
{
    /// <summary>
    ///     This class implements all logic revolving around the immediate finding of models in the model folder,<br />
    ///     watching for changes, serialization for transport and so on.
    /// </summary>
    internal class ModelManagementUseCase {
        private TModelDescription _currentModel;

        public ModelManagementUseCase(SimulationManagerSettings settings) {
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


        public JObject GetScenarioConfig(TModelDescription model, string scenarioConfigId)
        {
            var smServiceHost = "";
            try
            {
                var marsConfigService = new ConfigServiceClient(MARSConfigServiceSettings.Address);
                smServiceHost = marsConfigService.Get("scenario-management-service/host");
            }
            catch (Exception)
            {
                // ignored, means service not reachable or key not present
                Console.WriteLine("MARS Config Service not reachable, will try ServiceDiscovery with Eureka as fallback.");
            }

            if (smServiceHost == Empty)
            {
                var eureka = new EurekaClient();
                var smService = eureka.GetInstancesForApplication("SCENARIO-MANAGEMENT-SERVICE").FirstOrDefault();
                if (smService == null)
                {
                    throw new Exception(
                        "No ScenarioManagementService could be found in MARS Cloud. LIFE is shutting down now :(");
                }
                smServiceHost = $"http://{smService.IpAddress}:{smService.Port}/";
            }
        
            var http = new HttpClient();
            Console.WriteLine("Retrieving ScenarioConfig...");
            var uri = new Uri($"{smServiceHost}scenario-management/scenarios/{scenarioConfigId}/complete");
            var getTask = http.GetAsync(uri);
            getTask.Wait();
            if (getTask.Result.StatusCode != HttpStatusCode.OK)
            {
                throw new CouldNotGetScenarioConfigurationFromSMServiceException(
                    $"Failed to retrieve ScenarioConfiguration by URI: {uri.AbsoluteUri}." +
                    $"Did you run your Simulationcontainer inside of the MARS Cloud?");
            }
            var readAsString = getTask.Result.Content.ReadAsStringAsync();
            readAsString.Wait();


            return JObject.Parse(readAsString.Result);
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