using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using RuntimeEnvironment.Implementation.Entities;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("SimulationManagerTest")]

namespace RuntimeEnvironment.Implementation {

    internal enum SimulationStatus {
        Running,
		Stepped,
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
        private int? _steppedTicks;

        public SteppedSimulationExecutionUseCase
		(int? nrOfTicks, IList<LayerContainerClient> layerContainerClients, bool startPaused = false) {
            _nrOfTicks = nrOfTicks;
            _layerContainerClients = layerContainerClients;
            _status = startPaused ? SimulationStatus.Paused : SimulationStatus.Running;
            _simulationExecutionSwitch = new ManualResetEvent(false);

            // start simulation
            Task.Run(() => this.RunSimulation());
        }

        private void RunSimulation() {

            var sw = Stopwatch.StartNew();
            for (var i = 0; _nrOfTicks == null || i < _nrOfTicks; i++) {

                // check for status change
                switch (_status) {
                    case SimulationStatus.Paused:
                        // pause execution and wait to be signaled
                        sw.Stop();
                        _simulationExecutionSwitch.WaitOne();
                        sw.Start();
                        break;

					case SimulationStatus.Stepped:
                        if (_steppedTicks.HasValue){
                            while ((_steppedTicks+i < _nrOfTicks) && _steppedTicks > 0) {
                                DoStep();
                                _steppedTicks--;
                                i++;
                            }
                            _steppedTicks = null;
                        }
                        else {
                            DoStep();
                        }

                        // set switch to non-signaled in case it was signaled before
                        _simulationExecutionSwitch.Reset();

						// pause execution and wait to be signaled
                        sw.Stop();
						_simulationExecutionSwitch.WaitOne();
                        sw.Start();

						continue;

                    case SimulationStatus.Aborted:
                        // that's it..
                        return;
                }

				DoStep();

            }
			sw.Stop();
			Console.WriteLine ("Executed " + _nrOfTicks + " Ticks in " + sw.ElapsedMilliseconds * 1000);
        }

		private void DoStep(){

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

		public void StepSimulation(int? nrOfTicks = null) {
			_status = SimulationStatus.Stepped;
		    _steppedTicks = nrOfTicks;
			// signal ManualResetEvent
			_simulationExecutionSwitch.Set();
		}

        public void PauseSimulation() {
            // set switch to non-signaled in case it was signaled before
            _simulationExecutionSwitch.Reset();
            _status = SimulationStatus.Paused;
        }

        internal void ResumeSimulation() {
            _status = SimulationStatus.Running;

            // signal ManualResetEvent
            _simulationExecutionSwitch.Set();
        }

        public void Abort() {
            _status = SimulationStatus.Aborted;
        }

        public void StartVisualization(int? nrOfTicksToVisualize) {
            Parallel.ForEach(_layerContainerClients, l => l.Proxy.StartVisualization(nrOfTicksToVisualize));
        }

        public void StopVisualization()
        {
            Parallel.ForEach(_layerContainerClients, l => l.Proxy.StopVisualization());
        }
    }
}