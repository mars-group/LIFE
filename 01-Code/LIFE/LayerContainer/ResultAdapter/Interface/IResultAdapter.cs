//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using LifeAPI.Results;

namespace ResultAdapter.Interface {
  
  /// <summary>
  ///   The internal interface for the visualization adapter. Its method's won't be avaibale via the ILayercontainer interface.
  /// </summary>
  public interface IResultAdapter {
    
    /// <summary>
    ///   The Simulation ID. It will be set before the first call to WriteResults().
    /// </summary>
    /// <value>The simulation identifier.</value>    
    Guid SimulationId { get; set; }


    /// <summary>
    ///   Fetch all tick results and write them to the database.
    /// </summary>
    /// <param name="currentTick">The current tick. Needed for sanity check.</param>
    void WriteResults(int currentTick);


    /// <summary>
    ///   Register a simulation object at the result adapter. 
    /// </summary>
    /// <param name="simObject">The simulation entity to add to output queue.</param>
    void Register(ISimResult simObject);


    /// <summary>
    ///   Deregisters a simulation object from the result adapter. 
    /// </summary>
    /// <param name="simObject">The simulation entity to remove.</param>
    void DeRegister(ISimResult simObject);
  }
}