//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
namespace LIFE.API.Layer.TimeSeries {

  public interface ITimeSeriesLayer : ISteppedLayer {

    /// <summary>
    ///   Get a value for the current simulation time.
    /// </summary>
    /// <returns>
    ///   The value for the current simulation time. Returns null, if the stored value is empty or explicit null.
    ///   Returns a NoDataFound object, if the there is no value for the requested simulation time
    /// </returns>
    object GetValueForCurrentSimulationTime();
  }
}