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
using ResultAdapter.Interface;

namespace ResultAdapter.Implementation {

  /// <summary>
  ///   Generic result adapter component, implementing the concrete adapter.
  /// </summary>
  public class ResultAdapterComponent : IResultAdapter {

    private readonly IResultAdapter _resultAdapterInternalUseCase;


    /// <summary>
    ///   The Simulation ID. It will be set before the first call to WriteResults().
    /// </summary>
    /// <value>The simulation identifier.</value>   
    public Guid SimulationId {
      get { return _resultAdapterInternalUseCase.SimulationId; }
      set { _resultAdapterInternalUseCase.SimulationId = value; }
    }


    /// <summary>
    ///   Create a new result adapter.
    /// </summary>
    public ResultAdapterComponent() {
      _resultAdapterInternalUseCase = new ResultAdapterUseCase();
    }


    /// <summary>
    ///   Fetch all tick results and write them to the database.
    /// </summary>
    /// <param name="currentTick">The current tick. Needed for sanity check.</param>
    public void WriteResults(int currentTick) {
      _resultAdapterInternalUseCase.WriteResults(currentTick);
    }


    /// <summary>
    ///   Register a simulation object at the result adapter. 
    /// </summary>
    /// <param name="simObject">The simulation entity to add to output queue.</param>
    public void Register(ISimResult simObject) {
      _resultAdapterInternalUseCase.Register(simObject);
    }


    /// <summary>
    ///   Deregisters a simulation object from the result adapter. 
    /// </summary>
    /// <param name="simObject">The simulation entity to remove.</param>
    public void DeRegister(ISimResult simObject) {
      _resultAdapterInternalUseCase.DeRegister(simObject);
    }
  }
}