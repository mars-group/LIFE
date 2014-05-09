using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using RuntimeEnvironment.Implementation.Entities;

[assembly: InternalsVisibleTo("SimulationManagerTest")]

namespace RuntimeEnvironment.Implementation {
    
    internal enum SimulationStatus {
        Running,
        Paused,
        Aborted
    }

    /// <summary>
    /// This class implements simulation execution with parallel ticks on each layer container.<br/>
    /// For each system tick, each layer container is ticked parallely and we wait until every container is finished
    /// before the next system tick starts.
    /// </summary>
    internal class SteppedSimulationExecutionUseCase {
        private readonly int? _nrOfTicks;
        private readonly IList<LayerContainerClient> _layerContainerClients;
        private long _maxExecutionTime;
        private SimulationStatus _status;
        private int _containersLeft;

        public SteppedSimulationExecutionUseCase
            (int? nrOfTicks, IList<LayerContainerClient> layerContainerClients) {
            _nrOfTicks = nrOfTicks;
            _layerContainerClients = layerContainerClients;
            _status = SimulationStatus.Running;

            new Thread(RunSimulation).Start();
        }

        private void RunSimulation() {
            for (int i = 0; _nrOfTicks == null || i < _nrOfTicks; i++) {
                
                Monitor.Wait(this);
                while (_status == SimulationStatus.Paused)
                {
                    Monitor.Wait(this);
                }

                if (_status == SimulationStatus.Aborted) return;

                _maxExecutionTime = 0;
                _containersLeft = _layerContainerClients.Count;

                foreach (LayerContainerClient layerContainerClient in _layerContainerClients) {
                    ThreadPool.QueueUserWorkItem
                        (delegate {
                            long tickExecutionTime = layerContainerClient.Tick();
                            lock (_layerContainerClients) {
                                if (tickExecutionTime > _maxExecutionTime) _maxExecutionTime = tickExecutionTime;
                                _containersLeft--;
                                if (_containersLeft <= 0) Monitor.PulseAll(this);
                            }
                        });
                }

                Console.WriteLine("Simulation step #" + i + " finished. Longest exceution time: " + _maxExecutionTime);
            }
        }

        public void PauseSimulation() {
            _status = SimulationStatus.Paused;
        }

        internal void ResumeSimulation() {
            _status = SimulationStatus.Running;
            Monitor.PulseAll(this);
        }

        public void Abort() {
            _status = SimulationStatus.Aborted;
            Monitor.PulseAll(this);
        }
    }
}