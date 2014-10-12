using System.Collections.Generic;
using System.Linq;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Environments;
using GenericAgentArchitecture.Movement;
using GenericAgentArchitecture.Perception;
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
        //new Grass(GetNewID(), this, GetRandomPosition());
      }
    }


    /* Data source functions: Information types and retrieval method. */
    public enum InformationTypes { Agents }     

    public override object GetData(int informationType, IGeometry geometry) {
      switch ((InformationTypes) informationType) {      
        case InformationTypes.Agents: {
          var map = new Dictionary<long, SpatialAgent>();
          var halo = (Halo) geometry;
          foreach (var agent in GetAllAgents()) {
            if (halo.IsInRange(agent.GetPosition()) &&
                halo.Position.GetDistance(agent.GetPosition()) > float.Epsilon) {
              map[agent.Id] = agent;
            }
          }
          return map;
        }       
        default: return null;
      }
    }
  }
}
