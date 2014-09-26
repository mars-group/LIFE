using TVector = CommonTypes.DataTypes.Vector;

namespace GenericAgentArchitecture.Movement {
  
  /// <summary>
  ///   This class holds all spatial data for an agent.
  /// </summary>
  public class MData {
    
    public readonly Vector Position;       // The agent's current position.
    public readonly Direction Direction;   // Pitch and yaw values.


    /// <summary>
    ///   Create a new spatial data set.
    /// </summary>
    /// <param name="pos">The initial position (common transport vector).</param>
    public MData(TVector pos) {
      Position = new Vector(pos.X, pos.Y, pos.Z);
      Direction = new Direction();  // Default facing is straight line northbound.
    }
  }
}
