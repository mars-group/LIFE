using GenericAgentArchitecture.Movement;
using LayerAPI.Interfaces;

namespace GenericAgentArchitecture.Perception {
  
  /// <summary>
  ///   An abstract halo representation. Each sensor has one object of it.
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


    /// <summary>
    ///   Check, if a given position is inside this perception range.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True, if position is in range, false otherwise.</returns>
    public abstract bool IsInRange(Vector position);

    public abstract CommonTypes.DataTypes.Vector GetPosition();
    public abstract CommonTypes.DataTypes.Vector GetDimensionQuad();
    public abstract CommonTypes.DataTypes.Vector GetDirectionOfQuad();
    public abstract bool IsInRange(CommonTypes.DataTypes.Vector position);
  }
}