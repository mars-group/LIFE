using System;
using LIFE.API.Agent;
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
    /// <param name="resultConfigId">ID of the result configuration to load.</param>
    public ResultAdapterComponent(string resultConfigId) {
      _resultAdapterInternalUseCase = new ResultAdapterUseCase(resultConfigId);
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
    /// <param name="executionGroup">Agent execution (and output) group.</param>
    public void Register(ITickClient simObject, int executionGroup = 1) {
      _resultAdapterInternalUseCase.Register(simObject, executionGroup);
    }


    /// <summary>
    ///   De-registers a simulation object from the result adapter. 
    /// </summary>
    /// <param name="simObject">The simulation entity to remove.</param>
    /// <param name="executionGroup">Agent execution (and output) group.</param>
    public void DeRegister(ITickClient simObject, int executionGroup = 1) {
      _resultAdapterInternalUseCase.DeRegister(simObject, executionGroup);
    }
  }
}
