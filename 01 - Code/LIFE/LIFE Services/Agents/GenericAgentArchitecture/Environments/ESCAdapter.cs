﻿using System;
using System.Collections.Concurrent;
using CommonTypes.TransportTypes;
using System.Collections.Generic;
using DalskiAgent.Movement;
using ESCTestLayer.Interface;
using GenericAgentArchitectureCommon.Interfaces;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace DalskiAgent.Environments {
    
  /// <summary>
  ///   This adapter provides ESC usage via generic IEnvironment interface. 
  /// </summary> 
  public class ESCAdapter : IEnvironment {

    private readonly IUnboundESC _esc;  // Environment Service Component (ESC) implementation.
    private readonly TVector _maxSize;  // The maximum entent (for auto placement).
    private readonly bool _gridMode;    // ESC auto placement mode: True: grid, false: continuous.

    // Object-geometry mapping. Inner class for write-protected spatial entity representation.
    private readonly ConcurrentDictionary<ISpatialObject, GeometryObject> _objects; 
    private class GeometryObject : ISpatialEntity {
      public IGeometry Bounds { get; set; }
      public ISpatialObject obj;                    //TODO das hier oder interface oben=?

      public GeometryObject() {
        Bounds = new Point(1f, 1f, 1f);  // Default hitbox is cube with size 1.
      }
      public Vector GetPosition() {
        return new Vector(        
          (float) Bounds.Coordinate.X,
          (float) Bounds.Coordinate.Y,
          (float) Bounds.Coordinate.Z);
      }
    }
    /*  OBJEKT; DAS VON ISPATIAL ERBT UND BOUNDS SOWIE RÜCKGABEMETHODEN 
     *  FÜR POS UND DIRection DRINNE HAT. dataaccessor anpassen dafür
     */


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
      _objects = new ConcurrentDictionary<ISpatialObject, GeometryObject>();
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
      bool success;
      if (dir == null) dir = new Direction();

      var geometry = new GeometryObject();
      acc = new DataAccessor(geometry.Bounds);
      _objects[obj] = geometry;

      if (pos != null) success = _esc.Add(geometry, new TVector(pos.X, pos.Y, pos.Z), dir.Yaw); 
      else success = _esc.AddWithRandomPosition(geometry, TVector.Origin, _maxSize, _gridMode);                     
      if (!success) throw new Exception("[ESCAdapter] AddObject(): Placement failed, ESC returned 'false'!");
    }


    /// <summary>
    ///   Remove an object from the environment.
    /// </summary>
    /// <param name="obj">The object to delete.</param>
    public void RemoveObject(ISpatialObject obj) {
      _esc.Remove(_objects[obj]);
      GeometryObject g;
      _objects.TryRemove(obj, out g);   
    }
    

    /// <summary>
    ///   Displace a spatial object given a movement vector.
    /// </summary>
    /// <param name="obj">The object to move.</param>
    /// <param name="movement">Movement vector.</param>
    /// <param name="dir">The object's heading. If null, movement heading is used.</param>
    public void MoveObject(ISpatialObject obj, Vector movement, Direction dir = null) {
      if (!_objects.ContainsKey(obj)) return;
      if (dir == null) _esc.Move(_objects[obj], new TVector(movement.X, movement.Y, movement.Z));
      else _esc.Move(_objects[obj], new TVector(movement.X, movement.Y, movement.Z), dir.Yaw);
    }


    /// <summary>
    ///   Retrieve all objects of this environment.
    /// </summary>
    /// <returns>A list of all objects.</returns>
    public List<ISpatialObject> GetAllObjects() {
      var objects = new List<ISpatialObject>();
      foreach (var entity in _esc.ExploreAll()) {
        if (entity is ISpatialObject) objects.Add((ISpatialObject) entity);
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
    /// <param name="spec">Information object describing which data to query.</param>
    /// <returns>An object representing the percepted information.</returns>
    public object GetData(ISpecificator spec) {
      return _esc.GetData(spec);
    }
  }
}
