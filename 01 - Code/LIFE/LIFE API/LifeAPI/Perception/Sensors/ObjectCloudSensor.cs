﻿using System;
using System.Collections.Generic;
using LifeAPI.Layer.Data;
using LifeAPI.Spatial;

namespace LifeAPI.Perception.Sensors {

  /// <summary>
  ///   A sensor for the object cloud data layer.
  /// </summary>
  /// <typeparam name="T">Layer data type.</typeparam>
  public class ObjectCloudSensor<T> {

    private readonly IObjectCloudLayer<T> _layer;  // Layer reference.
    
    /// <summary>
    ///   Position vector reference (needed for relative queries).
    /// </summary>
    public Vector Position = null; 
    
    /// <summary>
    ///   Direction vector reference (needed for relative queries).
    /// </summary>
    public Vector Direction = null;


    /// <summary>
    ///   Create a sensor for object cloud layers.
    /// </summary>
    /// <param name="layer">The data layer.</param>
    public ObjectCloudSensor(IObjectCloudLayer<T> layer) {
      _layer = layer;
    } 


    /// <summary>
    ///   Returns all objects of type T with at least one vertex within the given shape, 
    ///   relative to the agent's orientation.
    /// </summary>
    /// <param name="shape">Query shape.</param>
    /// <returns>Set of data objects in query range.</returns>
    public ICollection<T> GetAll(ISpatialObject shape) {
      if (Position == null || Direction == null) {
        throw new Exception("[ObjectCloudSensor] Error on GetAll(): Position / direction references not set!");
      }
      var shapequery = new ShapeQuery {
        Position = Position, 
        Orientation = Direction, 
        QueryMask = shape
      };
      return _layer.Query(shapequery);
    }    
    
    
    /// <summary>
    ///   Similar to GetAll, but uses the given orientation instead of the agent's.
    /// </summary>
    /// <param name="shape">Query shape.</param>
    /// <returns>Set of data objects in query range.</returns>
    public ICollection<T> GetAllAbsolute(ShapeQuery shape) {
      return _layer.Query(shape);
    }
  }
}
