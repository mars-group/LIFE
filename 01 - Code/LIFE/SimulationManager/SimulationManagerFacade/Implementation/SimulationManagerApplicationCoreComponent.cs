﻿using System;
using System.Collections.Generic;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Service;
using ModelContainer.Interfaces;
using NodeRegistry.Interface;
using RuntimeEnvironment.Interfaces;

using SimulationManagerFacade.Interface;
using SMConnector;
using SMConnector.TransportTypes;
using SimulationManagerShared;

namespace SimulationManagerFacade.Implementation {
    using CommonTypes.DataTypes;

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
        private IScsServiceApplication _server;

        public SimulationManagerApplicationCoreComponent(SimulationManagerSettings settings,
                                        IRuntimeEnvironment runtimeEnvironment,
                                        IModelContainer modelContainer,
                                        INodeRegistry nodeRegistry) {
            _runtimeEnvironment = runtimeEnvironment;
            _modelContainer = modelContainer;
            _nodeRegistry = nodeRegistry;

            _server = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(settings.NodeRegistryConfig.NodeEndPointPort));

            _server.AddService<ISimulationManager, SimulationManagerApplicationCoreComponent>(this);

            _server.Start();
        }
        
		//local option for simulation start - connected layer containers unknown. Use all and use standard implementation.
		public void StartSimulationWithModel(TModelDescription model, int? nrOfTicks = null) {
			_runtimeEnvironment.StartWithModel(model, _nodeRegistry.GetAllNodesByType(CommonTypes.Types.NodeType.LayerContainer), nrOfTicks);
		}

        public void StartSimulationWithModel(TModelDescription model, bool startPaused, int? nrOfTicks = null) {
            _runtimeEnvironment.StartWithModel(model, _nodeRegistry.GetAllNodesByType(CommonTypes.Types.NodeType.LayerContainer), nrOfTicks, startPaused);
        }

        public void StepSimulation(TModelDescription model, int? nrOfTicks = null)
        {
            _runtimeEnvironment.StepSimulation(model, _nodeRegistry.GetAllNodesByType(CommonTypes.Types.NodeType.LayerContainer), nrOfTicks);
        }

        #region RuntimeEnvironment delegation


        public void StartSimulationWithModel
            (TModelDescription model, ICollection<TNodeInformation> layerContainers, bool startPaused, int? nrOfTicks = null) {
                _runtimeEnvironment.StartWithModel(model, layerContainers, nrOfTicks, startPaused);
        }

        public void StepSimulation(TModelDescription model, ICollection<TNodeInformation> layerContainers, int? nrOfTicks = null) {
            _runtimeEnvironment.StepSimulation(model, layerContainers, nrOfTicks);
        }

        public void PauseSimulation(TModelDescription model) {
            _runtimeEnvironment.Pause(model);
        }

        public void ResumeSimulation(TModelDescription model) {
            _runtimeEnvironment.Resume(model);
        }

        public void AbortSimulation(TModelDescription model) {
            _runtimeEnvironment.Abort(model);
        }

        public void StartVisualization(TModelDescription model, int? nrOfTicksToVisualize = null) {
            _runtimeEnvironment.StartVisualization(model, nrOfTicksToVisualize);
        }

        public void StopVisualization(TModelDescription model) {
            _runtimeEnvironment.StopVisualization(model);
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            _runtimeEnvironment.SubscribeForStatusUpdate(statusUpdateAvailable);
        }

        #endregion

        #region ModelContainer delegation

        public void RegisterForModelListChange(Action callback)
        {
            _modelContainer.RegisterForModelListChange(callback);
        }

        public ICollection<TModelDescription> GetAllModels()
        {
            return _modelContainer.GetAllModels();
        }

        public TModelDescription AddModelFromDirectory(string filePath)
        {
            return _modelContainer.AddModelFromDirectory(filePath);
        }

        public void DeleteModel(TModelDescription model)
        {
            _modelContainer.DeleteModel(model);
        }

        #endregion
    }
}