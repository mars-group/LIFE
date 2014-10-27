using System;
using System.Linq;
using ESCTestLayer.Implementation;
using GenericAgentArchitecture.Environments;
using LayerAPI.Interfaces;
using Mono.Addins;
using GenericAgentArchitecture.Movement;
using PedestrianModel;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace AgentTester.Wolves
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

        private long _tick;            // Counter of current tick.    
        private Random _random;        // Random number generator.
        private long _idCounter;       // Agent ID counter. Auto-incremented on each Add() call.    
        private LayerEnvironment _env; // Environment object for spatial agents. 


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
            _idCounter = 0;
            _random = new Random();
            _env = new LayerEnvironment(null, registerAgentHandle, unregisterAgentHandle, this);

            // Create some initial agents.
            var max = 10f;
            var pedDimension = new Vector(1f, 1f);
            var pedDirection = new Direction();
            pedDirection.SetPitch(0f);
            pedDirection.SetYaw(0f);

            int pedestrianCount = 10;

            for (var i = 0; i < pedestrianCount; i++)
            {
                // Random position between (0,0) and (10,10).
                var startPos = new Vector((float)_random.NextDouble() * max, (float)_random.NextDouble() * max);
                var targetPos = new Vector((float)_random.NextDouble() * max, (float)_random.NextDouble() * max);
                new Pedestrian(_idCounter++, _env, startPos, pedDimension, pedDirection, targetPos);
            }

            // Obstacle with center (5,5) going from x=4.5 to x=5.5 and y=0 to y=10
            var obsPosition = new Vector(5f, 5f);
            var obsDimension = new Vector(1f, 10f);
            var obsDirection = new Direction();
            obsDirection.SetPitch(0f);
            obsDirection.SetYaw(0f);

            new Obstacle(_idCounter++, _env, obsPosition, obsDimension, obsDirection);

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