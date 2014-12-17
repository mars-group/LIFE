using System.Collections.Generic;
using LifeAPI.Spatial;

namespace DalskiAgent.Environments {
  
  /// <summary>
  ///   This interface declares functions needed for movement services.
  ///   It thereby enables abstraction from ESC specific methods.
  /// </summary>
  public interface IEnvironmentOld {
    
    /// <summary>
    ///   Add a new object to the environment.
    /// </summary>
      /// <param name="obj">The object to add.</param>
      /// <param name="collisionType">Collision type defines with whom the agent may collide.</param>
    /// <param name="pos">The objects's initial position.</param>
    /// <param name="acc">Read-only object for data queries.</param>
    /// <param name="dim">Dimension of the object. If null, then (1,1,1).</param>
    /// <param name="dir">Direction of the object. If null, then 0°.</param>
    void AddObject(ISpatialObject obj, CollisionType collisionType, Vector pos, out DataAccessor acc, Vector dim, Direction dir);


    /// <summary>
    ///   Remove an object from the environment.
    /// </summary>
    /// <param name="obj">The object to delete.</param>
    void RemoveObject(ISpatialObject obj);


    /// <summary>
    ///   Displace a spatial object given a movement vector.
    /// </summary>
    /// <param name="obj">The object to move.</param>
    /// <param name="movement">Movement vector.</param>
    /// <param name="dir">The object's heading. If null, movement heading is used.</param>
    /// <returns>Result of movement, contains all SpatialEntities, with that this agent collided.</returns>
    MovementResult MoveObject(ISpatialObject obj, Vector movement, Direction dir = null);


    /// <summary>
    ///   Retrieve all objects of this environment.
    /// </summary>
    /// <returns>A list of all objects.</returns>
    List<ISpatialObject> GetAllObjects();


    /// <summary>
    ///   This method allows execution of environment related code. 
    ///   It is only useful in sequential mode and is executed before any agent.
    ///   For layer execution, inherit layer from ITickClient and register at itself!
    /// </summary>
    void AdvanceEnvironment();
  }
}
