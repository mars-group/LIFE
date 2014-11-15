using DalskiAgent.Movement;

namespace DalskiAgent.Environments {
  
  /// <summary>
  ///   This is the "Read-Only Object Representation".
  ///   It is returned on registration and used for value reading access.
  /// </summary>
  public class DataAccessor {

    private readonly SpatialData _data;  // Data source object.


    /// <summary>
    ///   Create a new read-only object for spatial data.
    /// </summary>
    /// <param name="data">The data source for queries.</param>
    public DataAccessor(SpatialData data) {
      _data = data;
    }


    /// <summary>
    ///   GET access for the object's current position.
    /// </summary>
    public Vector Position {
      get {
        var x = _data.Position.X;
        var y = _data.Position.Y;
        var z = _data.Position.Z;
        return new Vector(x,y,z);     
      }      
    }


    /// <summary>
    ///   GET accessor for direction of object.
    /// </summary>
    public Direction Direction {
      get {
        var dir = new Direction();
        dir.SetPitch(_data.Direction.Pitch);
        dir.SetYaw(_data.Direction.Yaw);
        return dir;
      }
    }
  }
}
