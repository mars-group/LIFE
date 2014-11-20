using System;
using System.Collections.Generic;
using System.Linq;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Movement;
using DalskiAgent.Perception;
using GenericAgentArchitectureCommon.Interfaces;

namespace AgentTester.Wolves.Agents {

  /// <summary>
  ///   This grassland is home to sheeps and wolves ... and yes, 'grass'.
  /// </summary>
  internal class Grassland : Environment2D, IGenericDataSource {

    private readonly Random _random;    // Random number generator for grass spawning.
    private readonly IExecution _exec;  // Agent execution container reference.

    /// <summary>
    ///   Create a new grassland.
    /// </summary>
    /// <param name="exec">Agent execution unit.</param>
    public Grassland(SeqExec exec) : base(new Vector(30, 20)) {
      _random = new Random();
      _exec = exec;
      exec.SetEnvironment(this);
    }


    /// <summary>
    ///   In this simple scenario, there is no need for environmental evolution. 
    ///   Nevertheless, the spawning of some additional grass agents would be nice.
    /// </summary>
    public override void AdvanceEnvironment() {
      var grassCount = Objects.Keys.OfType<Grass>().Count();
      var create = _random.Next(50 + grassCount) < 20;
      if (create) new Grass(_exec, this, GetRandomPosition());
    }


    /// <summary>
    ///   Retrieve information from a data source.
    ///   Overrides GetData to provide additional "Grass" agent queries.
    /// </summary>
    /// <param name="spec">Information object describing which data to query.</param>
    /// <returns>An object representing the percepted information.</returns>
    public object GetData(ISpecificator spec) {
      
      if (!(spec is Halo)) throw new Exception(
        "[Environment2D] Error on GetData() specificator: Not of type 'Halo'!");
      var halo = (Halo) spec;

      switch ((InformationTypes) spec.GetInformationType()) {      
        
        case InformationTypes.AllAgents:
          var objects = new List<ISpatialObject>();
          foreach (var obj in GetAllObjects())
            if (halo.IsInRange(obj.GetPosition().GetTVector())) objects.Add(obj);
          return objects;

        case InformationTypes.Grass: {
          var grass = new List<ISpatialObject>();
          foreach (var obj in GetAllObjects().OfType<Grass>())
            if (halo.IsInRange(obj.GetPosition().GetTVector())) grass.Add(obj);
          return grass;
        }

        default: return null;
      }
    }
  }
}
