//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System.Linq;
using LifeAPI.Perception.Sensors;

namespace LifeAPI.Layer.Data {

  /// <summary>
  ///   Data layer capable of LINQ query evaluation.
  /// </summary>
  /// <typeparam name="T">Layer data type.</typeparam>
  public interface IQueryLayer<T> {


    /// <summary>
    ///   Creates a sensor for this layer and data type combination. 
    /// </summary>
    /// <returns>A sensor for this layer.</returns>
    QuerySensor<T> GetSensor();


    /// <summary>
    ///   Returns a set of layer data that matches the query.
    /// </summary>
    /// <param name="query">LINQ query.</param>
    /// <returns>Layer data subset.</returns>
    IQueryable<T> GetQuery(string query);
  }
}
