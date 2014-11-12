using System;
using System.Linq;
using ESCTestLayer.Implementation;
using LayerAPI.Interfaces;
using Mono.Addins;
using PedestrianModel.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Movement;
using DalskiAgent.Execution;
using PedestrianModel.Util;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace PedestrianModel
{

    /// <summary>
    ///   Data query information types.
    /// </summary>
    public enum InformationTypes { AllAgents, Obstacles, Pedestrians }


    /// <summary>
    ///   This layer implementation contains a pedestrian simulation..
    ///   It uses the Generic Agent Architecture and serves as an example for other agent models.
    /// </summary>
    public class AgentLayerImpl : ISteppedLayer, ITickClient
    {

        private long _tick;         // Counter of current tick.    
        private Random _random;     // Random number generator.   
        private IEnvironment _env;  // Environment object for spatial agents. 
        private IExecution _exec;   // Agent execution container reference.

        /// <summary>
        ///   Initializes this layer.
        /// </summary>
        /// <typeparam name="T">Object type of layer init data object.</typeparam>
        /// <param name="layerInitData">Generic layer init data object. Not used here!</param>
        /// <param name="registerAgentHandle">Delegate for agent registration function.</param>
        /// <param name="unregisterAgentHandle">Delegate for agent unregistration function.</param>
        /// <returns></returns>
        public bool InitLayer<T>(T layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            _tick = 0;
            _random = new Random();
            _env = new Environment2D(new Vector(1000, 1000));
            _exec = new LayerExec(registerAgentHandle, unregisterAgentHandle, this);

            int pedestrianCount = 5;
            ScenarioBuilder.CreateTestScenario(_exec, _env, pedestrianCount);

            // Register the layer itself for execution. The agents are registered by themselves.
            registerAgentHandle.Invoke(this, this);
            return true;
        }


        /// <summary>
        ///   This layer is also tickable to execute some functions.
        ///   It increases the tick counter.
        /// </summary>
        public void Tick()
        {
            // Nothing to do here in this case.
            _tick++;
        }


        /// <summary>
        ///   Returns the current tick.
        /// </summary>
        /// <returns>Current tick value.</returns>
        public long GetCurrentTick()
        {
            return _tick;
        }
    }
}