namespace LIFE.Components.ESC.SpatialAPI.Entities.Movement {

  /// <summary>
  ///   The collision type defines different groups for that a collision behaviour is defined. Therefore see the CollisionMatrix.
  /// </summary>
  public enum CollisionType {
    Ghost,
    SelfCollision,
    StaticEnvironment,
    MassiveAgent,
  }
}