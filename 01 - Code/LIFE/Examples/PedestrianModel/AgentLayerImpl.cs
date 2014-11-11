using System;
using System.Linq;
using ESCTestLayer.Implementation;
using LayerAPI.Interfaces;
using Mono.Addins;
using PedestrianModel.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Movement;
using DalskiAgent.Execution;

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

            // Obstacle with center (10,15) going from x=9.5 to x=10.5 and y=10 to y=20
            var obsPosition = new Vector(10f, 15f);
            var obsDimension = new Vector(1f, 10f, 0.4f); // same height as pedestrians
            var obsDirection = new Direction();
            obsDirection.SetPitch(0f);
            obsDirection.SetYaw(0f);

            // OBSTACLES HAVE TO BE CREATED BEFORE THE AGENTS!
            new Obstacle(_exec, _env, obsPosition, obsDimension, obsDirection);

            var random = new Random();
            // WALK agents are 0.4m x 0.4m x 0.4m
            var pedDimension = new Vector(0.4f, 0.4f, 0.4f);
            var pedDirection = new Direction();
            pedDirection.SetPitch(0f);
            pedDirection.SetYaw(0f);

            for (var i = 0; i < pedestrianCount; i++)
            {
                // Random position between (0,10) and (9,20)
                var startPos = new Vector((float)random.NextDouble() * 9, (float)random.NextDouble() * 10 + 10f);
                // Random position between (11,10) and (20,20)
                var targetPos = new Vector((float)random.NextDouble() * 9 + 11f, (float)random.NextDouble() * 10 + 10f);
                new Pedestrian(_exec, _env, "sim0", startPos, pedDimension, pedDirection, targetPos);
            }             

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