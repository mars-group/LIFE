﻿//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 18.12.2015
//  *******************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using RuntimeEnvironment.Implementation.Entities;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("SimulationManagerTest")]

namespace RuntimeEnvironment.Implementation
{

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
		private Task _simulationTask;
		private Guid _simulationId;



        public SteppedSimulationExecutionUseCase
		(int? nrOfTicks, IList<LayerContainerClient> layerContainerClients, Guid simulationId, bool startPaused = false) {
            _nrOfTicks = nrOfTicks;
            _layerContainerClients = layerContainerClients;
            _status = startPaused ? SimulationStatus.Paused : SimulationStatus.Running;
            _simulationExecutionSwitch = new ManualResetEvent(false);
			_simulationId = simulationId;


            // start simulation
			_simulationTask = Task.Run(() => RunSimulation());
        }

        private void RunSimulation() {
            Console.WriteLine($"[LIFE] Starting Simulation with ID: {_simulationId} for {_nrOfTicks} ticks.");
            // if there's only a single layercontainer, just run all ticks at once as a batch run!
            if (_layerContainerClients.Count == 1)
            {
                if (_nrOfTicks != null) DoBatchRun(_nrOfTicks.Value);
            }
            else
            {
                var sw = Stopwatch.StartNew();
                for (var i = 1; _nrOfTicks == null || i <= _nrOfTicks; i++)
                {

                    // check for status change
                    switch (_status)
                    {

                        case SimulationStatus.Paused:
                            // pause execution and wait to be signaled
                            sw.Stop();
                            _simulationExecutionSwitch.WaitOne();
                            sw.Start();
                            break;

                        case SimulationStatus.Stepped:
                            if (_steppedTicks.HasValue)
                            {
                                // make sure we don't step over the maximum nr of Ticks
                                while ((_steppedTicks + i < _nrOfTicks) && _steppedTicks > 0)
                                {
                                    DoStep(i);
                                    _steppedTicks--;
                                    // increase overall tick number
                                    i++;
                                }
                                _steppedTicks = null;
                            }
                            else
                            {
                                DoStep(i);
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

                    DoStep(i);



                    // clean up after simulation, by calling DisposeLayer on all IDisposableLayers
                    CleanUp();
                }

                sw.Stop();
                Console.WriteLine("Executed " + _nrOfTicks + " Ticks in " + sw.ElapsedMilliseconds / 1000 + " seconds. Or " + sw.ElapsedMilliseconds + " ms.");
            }
        }

        private void DoBatchRun(int nrOfTicks)
        {
            _layerContainerClients[0].Tick(nrOfTicks);
        }

        private void DoStep(int currentTick){

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

            var status = $"Finished tick {currentTick} in {_maxExecutionTime} ms.";
            Console.SetCursorPosition(0,Console.CursorTop);
			Console.Write(status);
		}

		private void CleanUp(){
			Parallel.ForEach<LayerContainerClient> (_layerContainerClients, lc => lc.CleanUp());
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

		public void WaitForSimulationToFinish ()
		{
			_simulationTask.Wait ();
		}

		private int GetUnixTimeStamp(){
			return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
		}
    }
}