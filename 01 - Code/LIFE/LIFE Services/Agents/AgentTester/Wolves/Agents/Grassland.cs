using System;
using System.Collections.Generic;
using System.Linq;
using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Movement;
using GenericAgentArchitectureCommon.Interfaces;
using LayerAPI.Interfaces;

namespace AgentTester.Wolves.Agents {

  /// <summary>
  ///   This grassland is home to sheeps and wolves ... and yes, 'grass'.
  /// </summary>
  internal class Grassland : Environment2D {

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
      var grassCount = Agents.Keys.OfType<Grass>().Count();
      var create = _random.Next(50 + grassCount) < 20;
      if (create) new Grass(_exec, this, GetRandomPosition());
    }


    /// <summary>
    ///   Retrieve information from a data source.
    ///   Overrides GetData to provide additional "Grass" agent queries.
    /// </summary>
    /// <param name="informationType">The information type to query.</param>
    /// <param name="geometry">The perceptable area.</param>
    /// <returns>An arbitrary object. In this case, an agent listing.</returns>
    public override object GetData(int informationType, IGeometry geometry) {
      switch ((InformationTypes) informationType) {      
        case InformationTypes.AllAgents:
          return base.GetData(0, geometry);

        case InformationTypes.Grass: {
          var list = (List<SpatialAgent>) base.GetData(0, geometry);
          return list.OfType<Grass>().ToList();
        }

        default: return null;
      }
    }
  }
}
