using System;
using CommonTypes.TransportTypes;

namespace GenericAgentArchitecture.Movement {
    
  /// <summary>
  ///   This class holds all spatial data for an agent.
  /// </summary>
  public class MData {
    
    public readonly Vector Position;       // The agent's current position.
    public readonly Vector Dimension;      // The physical dimension.
    public readonly Direction Direction;   // Pitch and yaw values.
    
    /// <summary>
    ///   Create a new spatial data set.
    /// </summary>
    /// <param name="pos">The initial position (common transport vector).</param>
    public MData(TVector pos) {
      if (pos == null) throw new Exception("[MData] Error on initialization: Position is 'null'!");
      Position = new Vector(pos.X, pos.Y, pos.Z);
      Direction = new Direction();         // Default facing is straight line northbound.
      Dimension = new Vector(1f, 1f, 1f);  // Default hitbox is cube with size 1.
    }
  }
}
