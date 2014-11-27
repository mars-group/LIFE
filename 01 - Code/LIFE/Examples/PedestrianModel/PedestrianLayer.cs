﻿using System;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using LayerAPI.Interfaces;
using Mono.Addins;
using PedestrianModel.Environment;
using PedestrianModel.Util;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace PedestrianModel {

    /// <summary>
    ///     This layer implementation contains a pedestrian simulation..
    ///     It uses the Generic Agent Architecture and serves as an example for other agent models.
    /// </summary>
    [Extension(typeof (ISteppedLayer))]
    public class PedestrianLayer : ISteppedActiveLayer {
        private long _tick; // Counter of current tick.  
        private IEnvironment _env; // Environment object for spatial agents. 
        private IExecution _exec; // Agent execution container reference.

        #region ISteppedActiveLayer Members

        /// <summary>
        ///     Initializes this layer.
        /// </summary>
        /// <typeparam name="T">Object type of layer init data object.</typeparam>
        /// <param name="layerInitData">Generic layer init data object. Not used here!</param>
        /// <param name="registerAgentHandle">Delegate for agent registration function.</param>
        /// <param name="unregisterAgentHandle">Delegate for agent unregistration function.</param>
        /// <returns></returns>
        public bool InitLayer<T>
            (T layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            _tick = 0;

            Console.WriteLine("Calculate collisions?\n1: Collisions on.\n2: Collisions off.");
            String decisionString = Console.ReadLine();
            int decisionInt;
            int.TryParse(decisionString, out decisionInt);
            if (decisionInt == 1) {
                Config.UsesESC = true;
                Console.WriteLine("Collisions on.");
            }
            else if (decisionInt == 2) {
                Config.UsesESC = false;
                Console.WriteLine("Collisions off.");
            }
            else {
                Console.WriteLine("Invalid input. Using default settings.");
            }

            if (Config.UsesESC) _env = new ObstacleEnvironmentESC();
            else _env = new ObstacleEnvironment2D();
            _exec = new LayerExec(registerAgentHandle, unregisterAgentHandle, this);

            ScenarioBuilder.CreateScenario(_exec, _env, Config.Scenario);

            // Register the layer itself for execution. The agents are registered by themselves.
            registerAgentHandle.Invoke(this, this);
            return true;
        }

        public void PreTick() {
            if (_env is ObstacleEnvironmentESC) {
                ObstacleEnvironmentESC escEnv = (ObstacleEnvironmentESC) _env;
                escEnv.AdvanceEnvironment();
            }
            _env.AdvanceEnvironment();
        }

        /// <summary>
        ///     This layer is also tickable to execute some functions.
        ///     It increases the tick counter.
        /// </summary>
        public void Tick() {
            Console.WriteLine("Tick: " + _tick);
        }

        public void PostTick() {
            _tick++;
        }

        /// <summary>
        ///     Returns the current tick.
        /// </summary>
        /// <returns>Current tick value.</returns>
        public long GetCurrentTick() {
            return _tick;
        }

        public void SetCurrentTick(long currentTick) {
            throw new NotImplementedException();
        }

        #endregion
    }

}