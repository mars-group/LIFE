using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DalskiAgent.Perception;
using GenericAgentArchitectureCommon.Interfaces;
using LayerAPI.Interfaces;
using Mono.Addins;
using PedestrianModel.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Movement;
using DalskiAgent.Execution;
using PedestrianModel.Environment;
using PedestrianModel.Util;
using PedestrianModel.Logging;
using PedestrianModel.Visualization;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace PedestrianModel
{
    /// <summary>
    ///   This layer implementation contains a pedestrian simulation..
    ///   It uses the Generic Agent Architecture and serves as an example for other agent models.
    /// </summary>
    [Extension(typeof (ISteppedLayer))]
    public class PedestrianLayer : ISteppedLayer, ITickClient
    {
        private long _tick;         // Counter of current tick.  
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
            _env = new ObstacleEnvironment();
            _exec = new LayerExec(registerAgentHandle, unregisterAgentHandle, this);

            new Thread(() =>
            {
                SimpleVisualization visualization = new SimpleVisualization(_env);
                ObstacleEnvironment.Visualization = visualization;
                Application.Run(visualization);
            }).Start();

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
            Console.WriteLine("Tick: " + _tick);
            if (ObstacleEnvironment.Visualization != null)
            {
                ObstacleEnvironment.Visualization.Invalidate();
            }
            ObstacleEnvironment.AgentLogger.Log(_env.GetAllObjects().OfType<Pedestrian>().ToList());
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