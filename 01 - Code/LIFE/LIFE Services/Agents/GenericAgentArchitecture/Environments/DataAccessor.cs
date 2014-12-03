using GenericAgentArchitectureCommon.Datatypes;
using SpatialCommon.Datatypes;

namespace DalskiAgent.Environments {
  
  /// <summary>
  ///   This is the "Read-Only Object Representation".
  ///   It is returned on registration and used for value reading access.
  /// </summary>
  public class DataAccessor {

      private readonly SpatialData _data;         // Data source object.
      private readonly GeometryObject _geometry;  // Geometry source object.
      private readonly RectObject _rectObject;  // RectObject source object.


    /// <summary>
    ///   Create a new read-only object for spatial data.
    /// </summary>
    /// <param name="data">The data source for queries.</param>
    public DataAccessor(SpatialData data) {
      _data = data;
    }


    /// <summary>
    ///   Create a new r/o object for IGeometry objects.
    /// </summary>
    /// <param name="geometry">Geometry source object.</param>
    public DataAccessor(GeometryObject geometry)
    {
        _geometry = geometry;
    }

    /// <summary>
    ///   Create a new r/o object for IGeometry objects.
    /// </summary>
    /// <param name="geometry">Geometry source object.</param>
    public DataAccessor(RectObject rectObject)
    {
        _rectObject = rectObject;
    }


    /// <summary>
    ///   GET access for the object's current position.
    /// </summary>
    public Vector Position {
      get {
        double x, y, z;
        if (_data != null) {
          x = _data.Position.X;
          y = _data.Position.Y;
          z = _data.Position.Z;          
        }
        else if (_geometry != null) {
            return _geometry.Shape.GetPosition().GetVector();
        }
        else {
            return _rectObject.Shape.GetPosition().GetVector();
        }
        if (double.IsNaN(z)) z = 0.0f;
        return new Vector(x, y, z);
      }      
    }


    /// <summary>
    ///   GET accessor for direction of object.
    /// </summary>
    public Direction Direction {
      get {
        var dir = new Direction();
        if (_data != null) {
          dir.SetPitch(_data.Direction.Pitch);
          dir.SetYaw(_data.Direction.Yaw);
        }
        else {
          dir.SetPitch(_geometry.Direction.Pitch);
          dir.SetYaw(_geometry.Direction.Yaw);          
        }
        return dir;
      }
    }


    /// <summary>
    ///   GET accessor for dimension.
    /// </summary>
    public Vector Dimension {
      get {
        double x, y, z;
        if (_data != null) {
          x = _data.Dimension.X;
          y = _data.Dimension.Y;
          z = _data.Dimension.Z;
        }
        else {
          x = _geometry.Geometry.EnvelopeInternal.Width;
          y = _geometry.Geometry.EnvelopeInternal.Height;
          z = 0;
        }

        return new Vector(x, y, z);
      } 
    }
  }
}
