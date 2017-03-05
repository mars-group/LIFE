using LIFE.API.Agent;

namespace LIFE.API.GridCommon
{
    public class GridPosition : GridCoordinate
    {

    public GridPosition(int x, int y) : base(x, y) {}

    /// <summary>
    ///   Returns the current yaw as grid orientation constant.
    /// </summary>
    public GridDirection GridDirection { get; set; }


    /// <summary>
    ///   Return the direction as yaw angle.
    /// </summary>
    /// <returns>Compass heading (0 - lt. 360°).</returns>
    public int GetHeading() {
      switch (GridDirection) {
        case GridDirection.Up:        return 0;
        case GridDirection.UpRight:   return 45;
        case GridDirection.Right:     return 90;
        case GridDirection.DownRight: return 135;
        case GridDirection.Down:      return 180;
        case GridDirection.DownLeft:  return 225;
        case GridDirection.Left:      return 270;
        case GridDirection.UpLeft:    return 315;
        default: return 0;
      }
    }
  }
}