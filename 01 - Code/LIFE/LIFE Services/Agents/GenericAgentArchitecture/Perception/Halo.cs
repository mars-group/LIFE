using CommonTypes.TransportTypes;
using DalskiAgent.Movement;
using GenericAgentArchitectureCommon.Datatypes;
using GenericAgentArchitectureCommon.Interfaces;

namespace DalskiAgent.Perception {

  /// <summary>
  ///   An abstract halo representation. Each sensor has one object of it.
  ///   It has a geometry describing its form and a vertex as center point. 
  /// </summary>
  public abstract class Halo : IGeometry {
        
    public readonly Vector Position; // The agent's centre.

    /// <summary>
    ///   Create a new halo.
    /// </summary>
    /// <param name="position">The agent's centre.</param>
    protected Halo(Vector position) {
      Position = position;
    }

    public TVector GetPosition() {
      return Position.GetTVector();
    }

    public abstract AABB GetAABB();

    public abstract bool IsInRange(TVector position);
  }
}