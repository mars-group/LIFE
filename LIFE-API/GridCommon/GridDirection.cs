namespace LIFE.API.GridCommon {
  
  /// <summary>
  ///   Direction enumeration for grid movement.
  /// </summary>
  public enum GridDirection {
    NotSet = -1,
    Up = 0,
    Right = 90,
    Down = 180,
    Left = 270,
    UpRight = 45,
    DownRight = 135,
    DownLeft = 225,
    UpLeft = 315
  }
}