using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DalskiAgent.Movement;
using GenericAgentArchitectureCommon.Datatypes;

namespace DalskiAgent.Environments {
 
  /// <summary>
  ///   This environment adds movement support to the generic one and contains SpatialAgents. 
  /// </summary>
  public class Environment2D : IEnvironment {
    
    private readonly Vector _boundaries;    // Env. extent, ranging from (0,0) to this point.
    private readonly bool _isGrid;          // Grid-based or continuous environment?    
    private readonly Random _random;        // Number generator for random placement.
    protected readonly ConcurrentDictionary<ISpatialObject, SpatialData> Objects;  // Object listing.


    /// <summary>
    ///   Create a new 2D environment.
    /// </summary>
    /// <param name="boundaries">Boundaries for the environment.</param>
    /// <param name="isGrid">Selects, if this environment is grid-based or continuous.</param>
    public Environment2D(Vector boundaries, bool isGrid = true) {
      _boundaries = boundaries;
      _random = new Random();
      Objects = new ConcurrentDictionary<ISpatialObject, SpatialData>();
      _isGrid = isGrid;
    }


    /// <summary>
    ///   Add a new object to the environment.
    /// </summary>
    /// <param name="obj">The object to add.</param>
    /// <param name="pos">The objects's initial position.</param>
    /// <param name="acc">Read-only object for data queries.</param>
    /// <param name="dim">Dimension of the object. If null, then (1,1,1).</param>
    /// <param name="dir">Direction of the object. If null, then 0°.</param>
    public void AddObject(ISpatialObject obj, Vector pos, out DataAccessor acc, Vector dim, Direction dir) {
      if (pos == null) pos = GetRandomPosition();
      else if (!CheckPosition(pos)) 
        throw new Exception("[Environment2D] Error on object placement: Specified position already in use!");
     
      // Create new spatial data object and set values or default values.
      var mdata = new SpatialData(pos);
      if (dir != null) mdata.Direction = dir;
      else             mdata.Direction = new Direction(); // Default facing is straight line northbound.
      if (dim != null) mdata.Dimension = dim;
      else             mdata.Dimension = new Vector(1f, 1f, 1f);
      
      acc = new DataAccessor(mdata);
      Objects[obj] = mdata;
    }


    /// <summary>
    ///   Remove an object from the environment.
    /// </summary>
    /// <param name="obj">The object to delete.</param>
    public void RemoveObject(ISpatialObject obj) {
      SpatialData m;
      Objects.TryRemove(obj, out m);     
    }


    /// <summary>
    ///   Displace a spatial object given a movement vector.
    /// </summary>
    /// <param name="obj">The object to move.</param>
    /// <param name="movement">Movement vector.</param>
    /// <param name="dir">The object's heading. If null, movement heading is used.</param>
    public void MoveObject(ISpatialObject obj, Vector movement, Direction dir = null) {

      // If object reference is valid, get new movement data.
      if (!Objects.ContainsKey(obj)) return;
      var newPos = Objects[obj].Position + movement;
      if (dir == null) {
        dir = new Direction();
        dir.SetDirectionalVector(movement);
      }

      // Check position. If valid, set new values.
      if (CheckPosition(newPos)) {
        Objects[obj].Position = newPos;
        Objects[obj].Direction = dir;
      }
    }


    /// <summary>
    ///   This function allows execution of environment-specific code.
    ///   The generic 2D environment does not use it. Later override possible.
    /// </summary>
    public virtual void AdvanceEnvironment() {}


    /// <summary>
    ///   Retrieve all objects of this environment.
    /// </summary>
    /// <returns>A list of all objects.</returns>
    public List<ISpatialObject> GetAllObjects() {
      return new List<ISpatialObject>(Objects.Keys);
    }


    /// <summary>
    ///   Returns a random position.
    /// </summary>
    /// <returns>A free position.</returns>
    public Vector GetRandomPosition() {
      if (_isGrid) {
        bool unique;
        Vector position;
        do {
          var x = _random.Next((int)_boundaries.X);
          var y = _random.Next((int)_boundaries.Y);
          position = new Vector(x, y);
          unique = true;
          foreach (var md in Objects.Values) {
            if (md.Position.Equals(position)) {
              unique = false;
              break;              
            }
          }
        } while (!unique);
        return position;
      }

      //TODO 
      throw new NotImplementedException();
    }


    /*
    /// <summary>
    ///   Generate a valid initial movement data container for some given criteria. 
    /// </summary>
    /// <param name="dim">Extents of object to place (ranging from (0,0,0) to this point).</param>
    /// <param name="dir">Direction of object. If not set, it's chosen randomly.</param>
    /// <param name="target">A wished starting position. It's tried to find a fitting position as close as possible.</param>
    /// <param name="maxRng">Maximum allowed range to target position. Ignored, if no target specified.</param>
    /// <returns>A movement data object meeting the given requirements. 'Null', if placement not possible!</returns>
    private SpatialData PlaceAtRandomFreePosition(Vector dim, Direction dir = null, Vector target = null, float maxRng = 1) {
      throw new NotImplementedException();
    }
    */


    /// <summary>
    ///   Check, if a position can be acquired.
    /// </summary>
    /// <param name="position">The intended position</param>
    /// <returns>True, if accessible, false, when not.</returns>
    private bool CheckPosition(Vector position) {
      if (_isGrid) {
        if (position.X < 0 || position.X >= _boundaries.X ||
            position.Y < 0 || position.Y >= _boundaries.Y) return false;        
      }
      //TODO Dimensional and directional checks needed!
      foreach (var md in Objects.Values) {
        if (md.Position.Equals(position)) return false;
      }
      return true;
    }

    /*
    /// <summary>
    ///   This function is used by sensors to gather data from this environment.
    ///   It contains a function for "0: Get all perceptible agents". Further refinement 
    ///   can be made by specific environments overriding this function. 
    /// </summary>
    /// <param name="spec">Information object describing which data to query.</param>
    /// <returns>An object representing the percepted information.</returns>
    public virtual object GetData(ISpecificator spec) {
      if (!(spec is Halo)) throw new Exception(
        "[Environment2D] Error on GetData() specificator: Not of type 'Halo'!");
      var halo = (Halo) spec;
      switch (spec.GetInformationType()) {
        case 0: { // Zero stands here for "all agents". Enum avoided, check it elsewhere!
          var objects = new List<ISpatialObject>();
          foreach (var obj in GetAllObjects())
            if (halo.IsInRange(obj.GetPosition().GetTVector())) objects.Add(obj);
          return objects;
        }

        // Throw exception, if wrong argument was supplied.
        default: throw new Exception(
          "[Environment2D] Error on GetData call. Queried '"+spec.GetInformationType()+"', though "+
          "only '0' is valid. Please make sure to override function in specific environment!");
      }
    }*/
  }
}
