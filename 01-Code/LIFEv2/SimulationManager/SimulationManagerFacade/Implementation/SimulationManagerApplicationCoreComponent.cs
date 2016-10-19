//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 18.12.2015
//  *******************************************************/
using System;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Service;
using LNSConnector.Interface;
using LNSConnector.TransportTypes;
using ModelContainer.Interfaces;
using NodeRegistry.Interface;
using RuntimeEnvironment.Interfaces;
using SimulationManagerFacade.Interface;
using SMConnector;
using SMConnector.TransportTypes;
using SimulationManagerShared;

namespace SimulationManagerFacade.Implementation {
    /// <summary>
    ///     This class represents the implementation of the simulation manager's application core.
    /// </summary>
    /// <remarks>
    ///     It forces the instantiation of all the program's components by requiering them in it's constructor.<br />
    ///     It does, in compliance with the QUASAR pattern mostly delegate calls to the internal components <br />
    ///     (If it is allowed to access them from the outside). Also one would find something like transaction-<br />
    ///     or access controls here. So far this is not yet implemented.
    /// </remarks>
    public class SimulationManagerApplicationCoreComponent : ScsService, ISimulationManagerApplicationCore {
        private readonly IRuntimeEnvironment _runtimeEnvironment;
        private readonly IModelContainer _modelContainer;
        private readonly INodeRegistry _nodeRegistry;
        private readonly ILayerNameService _layerNameService;
        private IScsServiceApplication _server;

        public SimulationManagerApplicationCoreComponent(SimulationManagerSettings settings,
                                        IRuntimeEnvironment runtimeEnvironment,
                                        IModelContainer modelContainer,
                                        INodeRegistry nodeRegistry,
                                        ILayerNameService layerNameService) {
            _runtimeEnvironment = runtimeEnvironment;
            _modelContainer = modelContainer;
            _nodeRegistry = nodeRegistry;
            _layerNameService = layerNameService;

			// Create and start RemoteService for LayerNameService
            _server = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(settings.NodeRegistryConfig.NodeEndPointPort));

            _server.AddService<ILayerNameService, SimulationManagerApplicationCoreComponent>(this);

            _server.Start();

			// create and start SM WebService
			//var simManagerWebservice = new SimulationManagerWebserviceComponent(this);

        }
        
		//local option for simulation start - connected layer containers unknown. Use all and use standard implementation.
        public void StartSimulationWithModel(Guid simulationId,TModelDescription model, int? nrOfTicks = null, string scenarioConfig = "", bool startPaused = false) {
            _runtimeEnvironment.StartWithModel(simulationId, model, _nodeRegistry.GetAllNodesByType(CommonTypes.Types.NodeType.LayerContainer), nrOfTicks, scenarioConfig, startPaused);
        }

        public TModelDescription GetModelDescription(string modelPath)
        {
            return _modelContainer.GetModelDescription(modelPath);
        }

        public void StepSimulation(TModelDescription model, int? nrOfTicks = null)
        {
            _runtimeEnvironment.StepSimulation(model, _nodeRegistry.GetAllNodesByType(CommonTypes.Types.NodeType.LayerContainer), nrOfTicks);
        }

        #region RuntimeEnvironment delegation


        public void PauseSimulation(TModelDescription model) {
            _runtimeEnvironment.Pause(model);
        }

        public void ResumeSimulation(TModelDescription model) {
            _runtimeEnvironment.Resume(model);
        }

        public void AbortSimulation(TModelDescription model) {
            _runtimeEnvironment.Abort(model);
        }

        public void WaitForSimulationToFinish(TModelDescription model)
        {
            _runtimeEnvironment.WaitForSimulationToFinish(model);
        }


        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            _runtimeEnvironment.SubscribeForStatusUpdate(statusUpdateAvailable);
        }

        #endregion

        #region ModelContainer delegation



        #endregion

        public TLayerNameServiceEntry ResolveLayer(Type layerType)
        {
            return _layerNameService.ResolveLayer(layerType);
        }

        public void RegisterLayer(Type layerType, TLayerNameServiceEntry layerNameServiceEntry)
        {
            _layerNameService.RegisterLayer(layerType, layerNameServiceEntry);
        }

        public void RemoveLayer(Type layerType, TLayerNameServiceEntry layerNameServiceEntry)
        {
            _layerNameService.RemoveLayer(layerType, layerNameServiceEntry);
        }
    }
}