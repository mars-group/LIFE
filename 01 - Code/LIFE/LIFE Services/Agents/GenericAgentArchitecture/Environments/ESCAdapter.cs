using System;
using CommonTypes.TransportTypes;
using System.Collections.Generic;
using DalskiAgent.Movement;
using ESCTestLayer.Interface;
using GenericAgentArchitectureCommon.Interfaces;

namespace DalskiAgent.Environments {
    
  /// <summary>
  ///   This adapter provides ESC usage via generic IEnvironment interface. 
  /// </summary> 
  public class ESCAdapter : IEnvironment {

    private readonly IUnboundESC _esc;  // Environment Service Component (ESC) implementation.
    private readonly TVector _maxSize;  // The maximum entent (for auto placement).
    private readonly bool _gridMode;    // ESC auto placement mode: True: grid, false: continuous.


    /// <summary>
    ///   Create a new ESC adapter.
    /// </summary>
    /// <param name="esc">The ESC reference.</param>
    /// <param name="maxSize">The maximum entent (for auto placement).</param>
    /// <param name="gridMode">ESC auto placement mode: True: grid, false: continuous.</param>
    public ESCAdapter(IUnboundESC esc, Vector maxSize, bool gridMode) {
      _esc = esc;   
      _gridMode = gridMode;
      _maxSize = new TVector(maxSize.X, maxSize.Y, maxSize.Z);
    }


    /// <summary>
    ///   Add a new object to the environment.
    /// </summary>
    /// <param name="obj">The object to add.</param>
    /// <param name="pos">The objects's initial position.</param>
    /// <param name="acc">Read-only object for data queries.</param>
    /// <param name="dir">Direction of the object. If null, then 0°.</param>
    public void AddObject(IObject obj, Vector pos, out DataAccessor acc, Direction dir = null) {
      bool success;
      if (dir == null) dir = new Direction();
      if (pos != null) success = _esc.Add(obj, new TVector(pos.X, pos.Y, pos.Z), dir.Yaw); 
      else success = _esc.AddWithRandomPosition(obj, TVector.Origin, _maxSize, _gridMode);                     
      if (!success) throw new Exception("[ESCAdapter] AddObject(): Placement failed, ESC returned 'false'!");
      acc = new DataAccessor(null); //TODO
    }


    /// <summary>
    ///   Remove an object from the environment.
    /// </summary>
    /// <param name="obj">The object to delete.</param>
    public void RemoveObject(IObject obj) {
      _esc.Remove(obj);
    }
    

    /// <summary>
    ///   Displace a spatial object given a movement vector.
    /// </summary>
    /// <param name="obj">The object to move.</param>
    /// <param name="movement">Movement vector.</param>
    /// <param name="dir">The object's heading. If null, movement heading is used.</param>
    public void MoveObject(IObject obj, Vector movement, Direction dir = null) {
      if (dir == null) _esc.Move(obj, new TVector(movement.X, movement.Y, movement.Z));
      else _esc.Move(obj, new TVector(movement.X, movement.Y, movement.Z), dir.Yaw);
    }


    /// <summary>
    ///   Retrieve all objects of this environment.
    /// </summary>
    /// <returns>A list of all objects.</returns>
    public List<IObject> GetAllObjects() {
      var objects = new List<IObject>();
      foreach (var entity in _esc.ExploreAll()) {
        if (entity is IObject) objects.Add((IObject) entity);
      }
      return objects;
    }


    /// <summary>
    ///   Environment-related functions. Not needed in the ESC (at least, not now)!
    /// </summary>
    public void AdvanceEnvironment() { }


    /// <summary>
    ///   This function is used by sensors to gather data from this environment.
    ///   In this case, the adapter redirects to the ESC implementation.
    /// </summary>
    /// <param name="informationType">The type of information to sense.</param>
    /// <param name="deprecatedGeometry">The perception range.</param>
    /// <returns>An object representing the percepted information.</returns>
    public object GetData(int informationType, IDeprecatedGeometry deprecatedGeometry) {
      //TODO Geometrie umschreiben!
      return _esc.GetData(informationType, deprecatedGeometry);
    }
  }
}
