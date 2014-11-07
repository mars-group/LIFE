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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        var env = CreateScenarioEnvironment(exec, false, 10);
        exec.Run(1000, null);
    }

    private static IEnvironment CreateScenarioEnvironment(SeqExec exec, bool esc, int pedestrianCount)
    {
        IEnvironment env;
        if (!esc) env = new ObstacleEnvironment(exec);
        else env = new ESCAdapter(new ESC());

        // Obstacle with center (5,5) going from x=4.5 to x=5.5 and y=0 to y=10
        var obsPosition = new Vector(5f, 5f);
        var obsDimension = new Vector(1f, 10f);
        var obsDirection = new Direction();
        obsDirection.SetPitch(0f);
        obsDirection.SetYaw(0f);

        // OBSTACLES HAVE TO BE CREATED BEFORE THE AGENTS!
        new Obstacle(exec, env, obsPosition, obsDimension, obsDirection);

        var random = new Random();
        var max = 10f;
        var pedDimension = new Vector(1f, 1f);
        var pedDirection = new Direction();
        pedDirection.SetPitch(0f);
        pedDirection.SetYaw(0f);        

        for (var i = 0; i < pedestrianCount; i++)
        {
            // Random position between (0,0) and (10,10).
            var startPos = new Vector((float)random.NextDouble() * max, (float)random.NextDouble() * max);
            var targetPos = new Vector((float)random.NextDouble() * max, (float)random.NextDouble() * max);
            new Pedestrian(exec, env, "sim0", startPos, pedDimension, pedDirection, targetPos);
        }        

        return env;
    }
  }
}
