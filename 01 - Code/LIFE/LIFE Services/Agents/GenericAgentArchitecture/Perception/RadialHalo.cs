using GenericAgentArchitecture.Movement;

namespace GenericAgentArchitecture.Perception {
  
  /// <summary>
  ///   A halo capable of sensing in a circle around its position.
  /// </summary>
  public class RadialHalo : Halo {
    
    private readonly float _radius; // The radius describing the range of this halo.


    /// <summary>
    ///   Create a circular halo.
    /// </summary>
    /// <param name="position">The agent's centre.</param>
    /// <param name="radius">The radius describing the range of this halo.</param>
    public RadialHalo(Vector position, float radius) : base(position) {
      _radius = radius;
    }


    /// <summary>
    ///   Check, if a given position is in perception range.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>Radius check result.</returns>
    public override bool IsInRange(Vector position) {
      return Position.GetDistance(position) <= _radius;
    }



    public override CommonTypes.DataTypes.Vector GetPosition() {
      throw new System.NotImplementedException();
    }

    public override CommonTypes.DataTypes.Vector GetDimensionQuad() {
      throw new System.NotImplementedException();
    }

    public override CommonTypes.DataTypes.Vector GetDirectionOfQuad() {
      throw new System.NotImplementedException();
    }

    public override bool IsInRange(CommonTypes.DataTypes.Vector position) {
      throw new System.NotImplementedException();
    }
  }
}