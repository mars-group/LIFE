using System.Collections.Generic;
using DalskiAgent.Movement;
using GenericAgentArchitectureCommon.Interfaces;

namespace DalskiAgent.Environments {
  
  /// <summary>
  ///   This interface declares functions needed for movement services.
  ///   It thereby enables abstraction from ESC specific methods.
  /// </summary>
  public interface IEnvironment : IGenericDataSource {
    
    
    /// <summary>
    ///   Add a new object to the environment.
    /// </summary>
    /// <param name="obj">The object to add.</param>
    /// <param name="pos">The objects's initial position.</param>
    /// <param name="acc">Read-only object for data queries.</param>
    /// <param name="dir">Direction of the object. If null, then 0°.</param>
    void AddObject(IObject obj, Vector pos, out DataAccessor acc, Direction dir = null);


    /// <summary>
    ///   Remove an object from the environment.
    /// </summary>
    /// <param name="obj">The object to delete.</param>
    void RemoveObject(IObject obj);


    /// <summary>
    ///   Displace a spatial object given a movement vector.
    /// </summary>
    /// <param name="obj">The object to move.</param>
    /// <param name="movement">Movement vector.</param>
    /// <param name="dir">The object's heading. If null, movement heading is used.</param>
    void MoveObject(IObject obj, Vector movement, Direction dir = null);


    /// <summary>
    ///   Retrieve all objects of this environment.
    /// </summary>
    /// <returns>A list of all objects.</returns>
    List<IObject> GetAllObjects();


    /// <summary>
    ///   This method allows execution of environment related code. 
    ///   It is only useful in sequential mode and is executed before any agent.
    ///   For layer execution, inherit layer from ITickClient and register at itself!
    /// </summary>
    void AdvanceEnvironment();
  }
}
