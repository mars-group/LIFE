using CommonTypes.TransportTypes;
using DalskiAgent.Environments;
using GenericAgentArchitectureCommon.Interfaces;

namespace DalskiAgent.Perception {

  /// <summary>
  ///   An abstract halo representation. Each sensor has one object of it.
  ///   It has a geometry describing its form and a vertex as center point. 
  /// </summary>
  public abstract class Halo : IHalo {
        
    protected readonly DataAccessor Data; // The agent's R/O data container.


    /// <summary>
    ///   Create a new halo.
    /// </summary>
    /// <param name="data">The agent's R/O data container.</param>
    protected Halo(DataAccessor data) {
      Data = data;
    }


    /// <summary>
    ///   Check, if a given position is inside this perception range.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True, if position is in range, false otherwise.</returns>
    public abstract bool IsInRange(TVector position);
  }
}