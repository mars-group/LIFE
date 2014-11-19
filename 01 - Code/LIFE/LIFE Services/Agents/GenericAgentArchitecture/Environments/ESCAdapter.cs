﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ESCTestLayer.Implementation;
using ESCTestLayer.Interface;
using GenericAgentArchitectureCommon.Datatypes;
using GenericAgentArchitectureCommon.Interfaces;
using GenericAgentArchitectureCommon.TransportTypes;
using GeoAPI.Geometries;

namespace DalskiAgent.Environments {
    
  /// <summary>
  ///   This adapter provides ESC usage via generic IEnvironment interface. 
  /// </summary> 
  public class ESCAdapter : IEnvironment, IGenericDataSource {

    private readonly IUnboundESC _esc;  // Environment Service Component (ESC) implementation.
    private readonly TVector _maxSize;  // The maximum entent (for auto placement).
    private readonly bool _gridMode;    // ESC auto placement mode: True: grid, false: continuous.

    // Object-geometry mapping. Inner class for write-protected spatial entity representation.
    private readonly ConcurrentDictionary<ISpatialObject, GeometryObject> _objects; 


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
      if (dir == null) dir = new Direction();

      var geometry = new GeometryObject(MyGeometryFactory.Rectangle(dim.X, dim.Y), dir);
      acc = new DataAccessor(geometry);

      _objects[obj] = geometry;

      bool success;
      if (pos != null) success = _esc.Add(geometry, new TVector(pos.X, pos.Y, pos.Z), dir.GetDirectionalVector().GetTVector()); 
      else             success = _esc.AddWithRandomPosition(geometry, TVector.Origin, _maxSize, _gridMode);                     
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
      else _esc.Move(_objects[obj], new TVector(movement.X, movement.Y, movement.Z), dir.GetDirectionalVector().GetTVector());
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


  /// <summary>
  ///   This geometry class serves as a wrapper for the IGeometry object and its orientation.
  /// </summary>
  public class GeometryObject : ISpatialEntity {
    
    public IGeometry Geometry  { get; set; }  // Geometry to hold.
    public Direction Direction { get; set; }  // Direction object.


    /// <summary>
    ///   Create a geometry object.
    /// </summary>
    /// <param name="geo">Geometry to hold.</param>
    /// <param name="dir">Direction object.</param>
    public GeometryObject(IGeometry geo, Direction dir) {
      Geometry = geo;
      Direction = dir;
    }
  }
}
