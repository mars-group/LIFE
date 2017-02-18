//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using LIFE.API.Perception.Sensors;

namespace LIFE.API.Layer.Data {
  
  /// <summary>
  ///   Data layer type for single-value use cases.
  /// </summary>
  /// <typeparam name="T">Layer data type.</typeparam>
  public interface ISingleValueLayer<T> {


    /// <summary>
    ///   Creates a sensor for this layer and data type combination. 
    /// </summary>
    /// <returns>A sensor for this layer.</returns>
    SingleValueSensor<T> GetSensor();
      
      
    /// <summary>
    ///   Returns the current value of the layer.
    /// </summary>
    /// <returns>Layer value.</returns>
    T GetValue();
  }
}
