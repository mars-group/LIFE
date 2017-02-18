namespace LIFE.Components.ESC.SpatialAPI.Common {
  /// <summary>
  ///   The collision matrix defines, how different CollisionTypes interact with each other in case of collision. The index
  ///   of this 2-dimensional-matrix correspond with the order of the CollisionTypes. The matching indices describe if the
  ///   types collide (true) or not (false).
  ///   For example Ghost (row/index = 0) with Ghost (column/index = 0) is false.
  ///   For example StaticEnvironment (row/index = 3) with StaticEnvironment (column/index = 3) is false.
  ///   For example StaticEnvironment (row/index = 3) with MassiveAgent (column/index = 4) is true, so collides.
  /// </summary>
  public class CollisionMatrix {

    public static bool[,] Get() {
      return new[,] {
        {false, false, false, false},
        {false, true, false, false},
        {false, false, false, true},
        {false, false, true, true}
      };
    }
  }
}