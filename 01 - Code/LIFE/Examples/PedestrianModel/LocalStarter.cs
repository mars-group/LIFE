using CommonTypes.TransportTypes;
using DalskiAgent.Auxiliary;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Movement;
using ESCTestLayer.Implementation;
using ESCTestLayer.Interface;
using LayerAPI.Interfaces;
using PedestrianModel.Agents;
using PedestrianModel.Environment;
using PedestrianModel.Util;
using PedestrianModel.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PedestrianModel
{
  /// <summary>
  ///   This class periodicly triggers the environment and thereby all agents.
  /// </summary>
  internal class LocalStarter {
    
    /// <summary>
    ///   Program entry. Creates some agents and starts them.
    /// </summary>
    public static void Main() {
        var exec = new SeqExec(true);
        var env = CreateScenarioEnvironment(exec, false, Config.Scenario);        
        exec.Run((int)Math.Round(Config.LengthOfTimestepsInMilliseconds), null);
    }

    private static IEnvironment CreateScenarioEnvironment(SeqExec exec, bool esc, ScenarioBuilder.ScenarioTypes scenario)
    {
        IEnvironment env;
        if (!esc) env = new ObstacleEnvironment(exec);
        else env = new ESCAdapter(new ESC());
        
        new Thread(() => {
            SimpleVisualization visualization = new SimpleVisualization(env);
            ObstacleEnvironment.Visualization = visualization;
            Application.Run(visualization);            
        }).Start();

        ScenarioBuilder.CreateScenario(exec, env, scenario);
        
        return env;
    }    

  }
}
