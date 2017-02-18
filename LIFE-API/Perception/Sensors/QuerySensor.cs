//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System.Linq;
using LIFE.API.Layer.Data;

namespace LIFE.API.Perception.Sensors {

  /// <summary>
  ///   Sensor for query data layers.
  /// </summary>
  /// <typeparam name="T">Layer data type.</typeparam>
  public class QuerySensor<T> {

    private readonly IQueryLayer<T> _layer; // Layer reference.


    /// <summary>
    ///   Create a sensor for LINQ query grids.
    /// </summary>
    /// <param name="layer">The data layer.</param>
    public QuerySensor(IQueryLayer<T> layer) {
      _layer = layer;
    }


    /// <summary>
    ///   Allows arbitrary queries with the LINQ interface.
    /// </summary>
    /// <param name="query">LINQ query.</param>
    /// <returns>Layer data subset.</returns>
    public IQueryable<T> GetQuery(string query) {
      return _layer.GetQuery(query);
    }
  }
}