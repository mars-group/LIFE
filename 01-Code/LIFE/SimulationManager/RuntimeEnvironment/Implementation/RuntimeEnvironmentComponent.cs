//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 18.12.2015
//  *******************************************************/
using System;
using System.Collections.Generic;
using CommonTypes.DataTypes;
using ModelContainer.Interfaces;
using NodeRegistry.Interface;
using RuntimeEnvironment.Interfaces;
using SMConnector;
using SMConnector.TransportTypes;
using SimulationManagerShared;

namespace RuntimeEnvironment.Implementation {
	public class RuntimeEnvironmentComponent : IRuntimeEnvironment {
        private readonly IRuntimeEnvironment _runtimeEnvironmentUseCase;

        public RuntimeEnvironmentComponent(SimulationManagerSettings settings,
            IModelContainer modelContainer,
            INodeRegistry layerRegistry) {
            _runtimeEnvironmentUseCase = new RuntimeEnvironmentUseCase(modelContainer, layerRegistry);
        }

        public void StartWithModel
            (Guid simulationId,TModelDescription model, ICollection<TNodeInformation> layerContainerNodes, int? nrOfTicks = null, string simConfigName = "SimConfig.json", bool startPaused = false) {
            _runtimeEnvironmentUseCase.StartWithModel(simulationId, model, layerContainerNodes, nrOfTicks, simConfigName, startPaused);
        }

        public void StepSimulation(TModelDescription model, ICollection<TNodeInformation> layerContainerNodes, int? nrOfTicks = null) {
            _runtimeEnvironmentUseCase.StepSimulation(model, layerContainerNodes, nrOfTicks);
        }

        public void Pause(TModelDescription model) {
            _runtimeEnvironmentUseCase.Pause(model);
        }

        public void Resume(TModelDescription model) {
            _runtimeEnvironmentUseCase.Resume(model);
        }

        public void Abort(TModelDescription model) {
            _runtimeEnvironmentUseCase.Abort(model);
        }

		public void WaitForSimulationToFinish (TModelDescription model)
		{
			_runtimeEnvironmentUseCase.WaitForSimulationToFinish (model);
		}

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            _runtimeEnvironmentUseCase.SubscribeForStatusUpdate(statusUpdateAvailable);
        }
    }
}