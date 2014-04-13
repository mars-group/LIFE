﻿using System;
using System.Collections.Generic;
using System.Linq;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using LCConnector.TransportTypes;
using ModelContainer.Interfaces;
using NodeRegistry.Interface;
using RuntimeEnvironment.Interfaces;
using Shared;
using SMConnector;
using SMConnector.Exceptions;
using SMConnector.TransportTypes;

namespace RuntimeEnvironment.Implementation {
    internal class RuntimeEnvironmentUseCase : IRuntimeEnvironment {
        private readonly SimulationManagerSettings _settings;
        private readonly IModelContainer _modelContainer;
        private readonly INodeRegistry _nodeRegistry;
        private readonly IDictionary<TModelDescription, SteppedSimulationExecutionUseCase> _steppedSimulations;
        private readonly ISet<NodeInformationType> _idleLayerContainers;
        private readonly ISet<NodeInformationType> _busyLayerContainers;

        public RuntimeEnvironmentUseCase(SimulationManagerSettings settings,
            IModelContainer modelContainer,
            INodeRegistry nodeRegistry) {
            _settings = settings;
            _modelContainer = modelContainer;

            _steppedSimulations = new Dictionary<TModelDescription, SteppedSimulationExecutionUseCase>();
            _idleLayerContainers = new HashSet<NodeInformationType>();
            _busyLayerContainers = new HashSet<NodeInformationType>();

            _nodeRegistry.SubscribeForNewNodeConnectedByType(NewNode, NodeType.LayerContainer);
        }

        public void StartWithModel(TModelDescription model, ICollection<NodeInformationType> layerContainers,
            int? nrOfTicks = null) {
            lock (this) {
                if (!layerContainers.All(l => _idleLayerContainers.Any(c => c.Equals(l))))
                    throw new LayerContainerBusyException();

                if (_steppedSimulations.ContainsKey(model)) throw new SimulationAlreadyRunningException();

                IList<TLayerDescription> instantiationOrder = _modelContainer.GetInstantiationOrder(model);

                foreach (var nodeInformationType in layerContainers) {
                    _steppedSimulations[model] = new SteppedSimulationExecutionUseCase(_modelContainer, model,
                        instantiationOrder, layerContainers, nrOfTicks);
                }
            }
        }

        public void Pause(TModelDescription model) {
            if (!_steppedSimulations.ContainsKey(model)) {
                return;
            }

            _steppedSimulations[model].PauseSimulation();
        }

        public void Resume(TModelDescription model) {
            if (!_steppedSimulations.ContainsKey(model))
            {
                return;
            }

            _steppedSimulations[model].ResumeSimulation();
        }

        public void Abort(TModelDescription model) {
            if (!_steppedSimulations.ContainsKey(model))
            {
                return;
            }

            _steppedSimulations[model].Abort();
        }

        public void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable) {
            
        }

        private void NewNode(NodeInformationType newnode)
        {
            lock (this)
            {
                _idleLayerContainers.Add(newnode);
            }

        }
    }
}