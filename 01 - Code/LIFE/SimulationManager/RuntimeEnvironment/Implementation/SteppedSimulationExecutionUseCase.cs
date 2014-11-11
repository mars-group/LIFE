using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using RuntimeEnvironment.Implementation.Entities;
using System.Threading.Tasks;

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

        private readonly ManualResetEvent _simulationExecutionSwitch;

        public SteppedSimulationExecutionUseCase
            (int? nrOfTicks, IList<LayerContainerClient> layerContainerClients) {
            _nrOfTicks = nrOfTicks;
            _layerContainerClients = layerContainerClients;
            _status = SimulationStatus.Running;
            _simulationExecutionSwitch = new ManualResetEvent(false);

            // start simulation
            Task.Run(() => this.RunSimulation());
        }

        private void RunSimulation() {
            // start Visualization on all layerContainers
            Parallel.ForEach(_layerContainerClients, l => l.Proxy.StartVisualization());

			var now = DateTime.Now;
            for (var i = 0; _nrOfTicks == null || i < _nrOfTicks; i++) {

                // check for status change
                switch (_status) {
                    case SimulationStatus.Paused:
                        // pause execution and wait to be signaled
                        _simulationExecutionSwitch.WaitOne();
                        break;

                    case SimulationStatus.Aborted:
                        // that's it..
                        return;
                }

                _maxExecutionTime = 0;

                // now for some .NET 4.5 magic: parallel execution of layerContainer.tick() while updating shared variable
                Parallel.ForEach<LayerContainerClient, long> // elem, accu
                    (
                        _layerContainerClients, //source for elems
                        () => 0, // intialization for accu
                        (currentContainer, loop, lastExecutionTime) => { // currentElem, ParallelLoopState, lastAccu
                            var currentExecutionTime = currentContainer.Tick(); // do actual simulation step
                            return Math.Max(currentExecutionTime, lastExecutionTime); 
                        },
                        (finalResult) => { // finalResult = final result from inner partitioned loop
                            // now read shared variable
                            long localMax = Interlocked.Read(ref _maxExecutionTime); 
                            // while finalResult is larger than _maxExecutionTime, update it, and try again
                            while (finalResult > localMax) {
                              Interlocked.CompareExchange(ref _maxExecutionTime, finalResult, localMax);
                              localMax = Interlocked.Read(ref _maxExecutionTime);
                            }
                        });


                //Console.WriteLine("Simulation step #" + i + " finished. Longest exceution time: " + _maxExecutionTime);
            }
			var then = DateTime.Now;
			Console.WriteLine ("Executed " + _nrOfTicks + " Ticks in " + (then-now).TotalSeconds);
        }

        public void PauseSimulation() {
            // set switch to non-signaled in case it was signaled before
            _simulationExecutionSwitch.Reset();
            _status = SimulationStatus.Paused;
        }

        internal void ResumeSimulation() {
            _status = SimulationStatus.Running;
            // signal ManualResetEvent
            this._simulationExecutionSwitch.Set();
        }

        public void Abort() {
            _status = SimulationStatus.Aborted;
        }
    }
}