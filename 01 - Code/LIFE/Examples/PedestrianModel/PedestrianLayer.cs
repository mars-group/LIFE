using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using LayerAPI.Interfaces;
using Mono.Addins;
using PedestrianModel.Agents;
using PedestrianModel.Environment;
using PedestrianModel.Util;
using PedestrianModel.Visualization;

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
            _env = new ObstacleEnvironment();
            _exec = new LayerExec(registerAgentHandle, unregisterAgentHandle, this);

            new Thread
                (() => {
                    SimpleVisualization visualization = new SimpleVisualization(_env);
                    ObstacleEnvironment.Visualization = visualization;
                    Application.Run(visualization);
                }).Start();

            ScenarioBuilder.CreateScenario(_exec, _env, Config.Scenario);

            // Register the layer itself for execution. The agents are registered by themselves.
            registerAgentHandle.Invoke(this, this);
            return true;
        }

        public void PreTick() {
            if (ObstacleEnvironment.Visualization != null) ObstacleEnvironment.Visualization.Invalidate();
            ObstacleEnvironment.AgentLogger.Log(_env.GetAllObjects().OfType<Pedestrian>().ToList());
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