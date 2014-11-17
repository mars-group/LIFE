using System;
using System.Collections.Generic;
using System.Linq;
using DalskiAgent.Perception;
using GenericAgentArchitectureCommon.Interfaces;
using LayerAPI.Interfaces;
using Mono.Addins;
using PedestrianModel.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Movement;
using DalskiAgent.Execution;
using PedestrianModel.Util;
using PedestrianModel.Logging;

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
    public class AgentLayerImpl : ISteppedLayer, ITickClient, IGenericDataSource
    {

        private long _tick;         // Counter of current tick.  
        private IEnvironment _env;  // Environment object for spatial agents. 
        private IExecution _exec;   // Agent execution container reference.

        private readonly AgentLogger agentLogger = new AgentLogger();

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
            _env = new Environment2D(new Vector(1000, 1000), false);
            _exec = new LayerExec(registerAgentHandle, unregisterAgentHandle, this);

            ScenarioBuilder.CreateScenario(_exec, _env, Config.Scenario);

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
            Console.WriteLine("Tick!");
            agentLogger.Log(_env.GetAllObjects().OfType<Pedestrian>().ToList());
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

        /// <summary>
        ///   Retrieve information from a data source.
        /// </summary>
        /// <param name="spec">Information object describing which data to query.</param>
        /// <returns>An object representing the percepted information.</returns>
        public object GetData(ISpecificator spec)
        {

            if (!(spec is Halo)) throw new Exception(
              "[Environment2D] Error on GetData() specificator: Not of type 'Halo'!");
            var halo = (Halo)spec;

            switch ((InformationTypes)spec.GetInformationType())
            {

                case InformationTypes.AllAgents:
                    var objects = new List<ISpatialObject>();
                    foreach (var obj in _env.GetAllObjects())
                        if (halo.IsInRange(obj.GetPosition().GetTVector())) objects.Add(obj);
                    return objects;

                case InformationTypes.Obstacles:
                    {
                        var obstacle = new List<Obstacle>();
                        foreach (var obj in _env.GetAllObjects().OfType<Obstacle>())
                            if (halo.IsInRange(obj.GetPosition().GetTVector())) obstacle.Add(obj);
                        return obstacle;
                    }

                case InformationTypes.Pedestrians:
                    {
                        var pedestrian = new List<Pedestrian>();
                        foreach (var obj in _env.GetAllObjects().OfType<Pedestrian>())
                            if (halo.IsInRange(obj.GetPosition().GetTVector())) pedestrian.Add(obj);
                        return pedestrian;
                    }

                default: return null;
            }
        }
    }
}