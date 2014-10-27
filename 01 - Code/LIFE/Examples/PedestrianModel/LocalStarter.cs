using CommonTypes.TransportTypes;
using ESCTestLayer.Implementation;
using ESCTestLayer.Interface;
using GenericAgentArchitecture.Auxiliary;
using GenericAgentArchitecture.Environments;
using GenericAgentArchitecture.Movement;
using LayerAPI.Interfaces;
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
    
    private readonly ITickClient _environment; // The agent container.
    private readonly ConsoleView _view;        // The console view module.


    /// <summary>
    ///   Instantiate a runtime.
    ///   <param name="environment">The environment to execute.</param>
    ///   <param name="view">The console view module.</param>
    /// </summary>
    private LocalStarter(ITickClient environment, ConsoleView view) {
      _environment = environment;
      _view = view;
    }


    /// <summary>
    ///   Execution routine. Sends a tick to the environment container.
    /// </summary>
    /// <param name="delay">Thread delay (in ms), 0 for manual execution.</param>
    private void Run(int delay) {
      while (true) {
        _environment.Tick();
        if (_view != null) _view.Print();

        // Manual or automatic execution.
        if (delay == 0) Console.ReadLine();
        else {
          Console.WriteLine();
          Thread.Sleep(delay);
        }
      }
    }

    
    /// <summary>
    ///   Program entry. Creates some agents and starts them.
    /// </summary>
    public static void Main() {
      //var environment = CreateScenarioEnvironment(10, new ESC());
      var environment = CreateScenarioEnvironment(10);
      var view = CreateConsoleView((ObstacleEnvironment) environment);
      new LocalStarter(environment, view).Run(0);
    }

    private static ConsoleView CreateConsoleView(ObstacleEnvironment environment)
    {
        throw new NotImplementedException();
    }

    private static ITickClient CreateScenarioEnvironment(int pedestrianCount, IESC esc = null)
    {
        var obstacleEnvironment = new ObstacleEnvironment { RandomExecution = false };
        IEnvironment env;

        // If ESC exists, create adapter and use it as position manager. Otherwise use internal.
        if (esc != null)
        {
            var adapter = new ESCAdapter(esc);
            env = adapter;
        }
        else env = obstacleEnvironment;

        var random = new Random();
        var max = 10f;
        var pedDimension = new Vector(1f, 1f);
        var pedDirection = new Direction();
        pedDirection.SetPitch(0f);
        pedDirection.SetYaw(0f);

        long idCounter = 0;

        for (var i = 0; i < pedestrianCount; i++)
        {            
            // Random position between (0,0) and (10,10).
            var startPos = new Vector((float)random.NextDouble() * max, (float)random.NextDouble() * max);
            var targetPos = new Vector((float)random.NextDouble() * max, (float)random.NextDouble() * max);
            new Pedestrian(idCounter++, env, startPos, pedDimension, pedDirection, targetPos);
        }

        // Obstacle with center (5,5) going from x=4.5 to x=5.5 and y=0 to y=10
        var obsPosition = new Vector(5f, 5f);
        var obsDimension = new Vector(1f, 10f);
        var obsDirection = new Direction();
        obsDirection.SetPitch(0f);
        obsDirection.SetYaw(0f);

        new Obstacle(idCounter++, env, obsPosition, obsDimension, obsDirection);
        

        return obstacleEnvironment;
    }
  }
}
