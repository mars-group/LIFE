using System;
using System.Threading;
using System.Windows.Forms;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using PedestrianModel.Environment;
using PedestrianModel.Util;
using PedestrianModel.Visualization;

namespace PedestrianModel {

    /// <summary>
    ///     This class periodicly triggers the environment and thereby all agents.
    /// </summary>
    internal class LocalStarter {
        /// <summary>
        ///     Program entry. Creates some agents and starts them.
        /// </summary>
        public static void Main() {
            SeqExec exec = new SeqExec(true);
            CreateScenarioEnvironment(exec, Config.UsesESC, Config.Scenario);
            exec.Run((int) Math.Round(Config.LengthOfTimestepsInMilliseconds), null);
        }

        private static void CreateScenarioEnvironment
            (SeqExec exec, bool usesESC, ScenarioType scenario) {
            ObstacleEnvironment env = new ObstacleEnvironment(exec, usesESC);

            new Thread
                (() => {
                    SimpleVisualization visualization = new SimpleVisualization(env);
                    ObstacleEnvironment.Visualization = visualization;
                    Application.Run(visualization);
                }).Start();

            ScenarioBuilder.CreateScenario(exec, env, scenario);
        }
    }

}