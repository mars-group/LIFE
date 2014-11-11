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
        var env = CreateScenarioEnvironment(exec, false, 5);        
        exec.Run(100, null);
    }

    private static IEnvironment CreateScenarioEnvironment(SeqExec exec, bool esc, int pedestrianCount)
    {
        IEnvironment env;
        if (!esc) env = new ObstacleEnvironment(exec);
        else env = new ESCAdapter(new ESC());
        
        new Thread(() => {
            Form visualization = new SimpleVisualization(env);
            ObstacleEnvironment.Visualization = visualization;
            Application.Run(visualization);            
        }).Start();

        // Obstacle with center (10,15) going from x=9.5 to x=10.5 and y=10 to y=20
        var obsPosition = new Vector(10f, 15f);
        var obsDimension = new Vector(1f, 10f, 0.4f); // same height as pedestrians
        var obsDirection = new Direction();
        obsDirection.SetPitch(0f);
        obsDirection.SetYaw(0f);

        // OBSTACLES HAVE TO BE CREATED BEFORE THE AGENTS!
        new Obstacle(exec, env, obsPosition, obsDimension, obsDirection);

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
            new Pedestrian(exec, env, "sim0", startPos, pedDimension, pedDirection, targetPos);
        }        

        return env;
    }
  }
}
