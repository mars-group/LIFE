using System.Collections.Generic;
using System.Linq;
using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Movement;
using LayerAPI.Interfaces;

namespace AgentTester.Wolves.Agents {

  /// <summary>
  ///   This grassland is home to sheeps and wolves ... and yes, 'grass'.
  /// </summary>
  internal class Grassland : Environment2D {
    
    /// <summary>
    ///   Create a new grassland.
    /// </summary>
    public Grassland() : base (new Vector(30, 18)) {}


    /// <summary>
    ///   In this simple scenario, there is no need for environmental evolution. 
    ///   Nevertheless, the spawning of some additional grass agents would be nice.
    /// </summary>
    protected override void AdvanceEnvironment() {
      var grassCount = Agents.Keys.OfType<Grass>().Count();
      if (Random.Next(40+grassCount) < 20) {
        new Grass(IDCounter, this, GetRandomPosition());
      }
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
