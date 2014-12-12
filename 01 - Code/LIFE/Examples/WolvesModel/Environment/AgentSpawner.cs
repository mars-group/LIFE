using System;
using System.Collections.Generic;
using System.Linq;
using DalskiAgent.Agents;
using DalskiAgent.Auxiliary;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Perception;
using DalskiAgent.Reasoning;
using EnvironmentServiceComponent.Implementation;
using LayerAPI.Perception;
using WolvesModel.Agents;

namespace WolvesModel.Environment {
  
  /// <summary>
  ///   This class is responsible for the creation of additional agents.
  /// </summary>
  internal class AgentSpawner : Agent, IAgentLogic {

    // Normally, you don't save this references. But in this case we need them to pass along.
    private readonly IExecution _exec;        // Execution reference.
    private readonly Grassland _env;          // Grassland reference.
    private readonly Random _random;          // Random number generator for agent spawning.
    public long TickCnt { get; private set; } // Read-only tick counter (increased in each cycle). 


    /// <summary>
    ///   Create a new agent spawner. It is capable of sense the 
    ///   other agents and can create some based on their counts. 
    /// </summary>
    /// <param name="exec">Execution environment</param>
    /// <param name="env">Grassland reference.</param>
    public AgentSpawner(IExecution exec, Grassland env) : base(exec) {
      _env = env;
      _exec = exec;
      _random = new Random();

      // Add perception sensor.
      ISpecification halo;
      if (env.UsesEsc) halo = new SpatialHalo(MyGeometryFactory.Rectangle(100, 100), InformationTypes.AllAgents);
      else             halo = new OmniHalo(InformationTypes.AllAgents);
      PerceptionUnit.AddSensor(new DataSensor(this, env, halo));
      TickCnt = 1;
      Init();
    }


    /// <summary>
    ///   Agent spawning logic.
    ///   It also carries an tick counter.
    /// </summary>
    /// <returns>Always 'null', because this agent does not interact.</returns>
    public IInteraction Reason() {

      // Output numbers.
      var raw = (List<ISpatialObject>) PerceptionUnit.GetData(InformationTypes.AllAgents).Data;
      var grass  = raw.OfType<Grass>().Count();
      //var sheeps = raw.OfType<Sheep>().Count();
      //var wolves = raw.OfType< Wolf>().Count();     
      //ConsoleView.AddMessage("["+GetTick()+"] Gras "+grass+", Schafe "+sheeps+", Wölfe "+wolves, ConsoleColor.DarkGray);

      // Grass spawning.
      var create = _random.Next(50 + grass*2) < 20;
      if (create) {
        var g = new Grass(_exec, _env);
        ConsoleView.AddMessage("["+GetTick()+"] Neues Gras auf Position "+g.GetPosition(), ConsoleColor.Cyan);
      }


      TickCnt ++;
      return null;
    }
  }
}
