using System;
using System.Collections.Generic;
using System.Threading;
using CommonTypes.DataTypes;
using Hik.Communication.ScsServices.Client;
using LCConnector;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using ModelContainer.Interfaces;
using RuntimeEnvironment.Implementation.Entities;
using SMConnector.TransportTypes;

namespace RuntimeEnvironment.Implementation {
    internal enum SimulationStatus {
        Running,
        Paused,
        Aborted
    }

    internal class SteppedSimulationExecutionUseCase {
        private readonly int? _nrOfTicks;
        private readonly IList<LayerContainerClient> _layerContainerClients;
        private long _maxExecutionTime;
        private SimulationStatus _status;
        private int _containersLeft;

        public SteppedSimulationExecutionUseCase(
            IModelContainer modelContainer,
            TModelDescription modelDescription,
            IList<TLayerDescription> instantiationOrder,
            ICollection<TNodeInformation> layerContainers,
            int? nrOfTicks) {
            _nrOfTicks = nrOfTicks;

            //connect to the layer containers and initialize layers
            ModelContent content = modelContainer.GetModel(modelDescription);
            _layerContainerClients = new LayerContainerClient[layerContainers.Count];

            int i = 0;
            foreach (var nodeInformationType in layerContainers) {
                _layerContainerClients[i] =
                    new LayerContainerClient(
                        ScsServiceClientBuilder.CreateClient<ILayerContainer>(
                            nodeInformationType.NodeEndpoint.IpAddress + ":" +
                            nodeInformationType.NodeEndpoint.Port),
                        content,
                        instantiationOrder,
                        i,
                        TickFinished);
                    
                i++;
            }

            _status = SimulationStatus.Running;

            new Thread(RunSimulation).Start();
        }

        private void TickFinished(long tickExecutionTime) {
            lock (_layerContainerClients) {
                if (tickExecutionTime > _maxExecutionTime) _maxExecutionTime = tickExecutionTime;
                _containersLeft--;
                if (_containersLeft <= 0) Monitor.PulseAll(this);
            }
        }

        private void RunSimulation() {
            for (int i = 0; _nrOfTicks == null || i < _nrOfTicks; i++) {
                _maxExecutionTime = 0;
                _containersLeft = _layerContainerClients.Count;

                foreach (var layerContainerClient in _layerContainerClients) {
                    Thread thread = new Thread(layerContainerClient.Tick);
                    thread.Start();
                }

                Monitor.Wait(this);
                if (_status == SimulationStatus.Aborted) return;

                while (_status == SimulationStatus.Paused) {
                    Monitor.Wait(this);
                }

                Console.WriteLine("Simulation step #" + i + " finished. Longest exceution time: " + _maxExecutionTime);
            }
        }

        public void PauseSimulation() {
            _status = SimulationStatus.Paused;
        }

        internal void ResumeSimulation()
        {
            _status = SimulationStatus.Running;
            Monitor.PulseAll(this);
        }

        public void Abort() {
            _status = SimulationStatus.Aborted;
            Monitor.PulseAll(this);
        }
    }
}