using CommonTypes.TransportTypes;
using DalskiAgent.Environments;
using GenericAgentArchitectureCommon.Interfaces;

namespace DalskiAgent.Perception {

  /// <summary>
  ///   An abstract halo representation. Each sensor has one object of it.
  ///   It has a geometry describing its form and a vertex as center point. 
  /// </summary>
  public abstract class Halo : ISpecificator {
        
    protected readonly DataAccessor Data;  // The agent's R/O data container.
    private readonly int _informationType; // The information type to query.

    /// <summary>
    ///   Create a new halo.
    /// </summary>
    /// <param name="data">The agent's R/O data container.</param>
    /// <param name="informationType">The information type to query.</param>
    protected Halo(DataAccessor data, int informationType) {
      Data = data;
      _informationType = informationType;
    }


    /// <summary>
    ///   Return the information type specified by this object.
    /// </summary>
    /// <returns>Information type (as enum value).</returns>
    public int GetInformationType() {
      return _informationType;
    }


    /// <summary>
    ///   Check, if a given position is inside this perception range.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True, if position is in range, false otherwise.</returns>
    public abstract bool IsInRange(TVector position);
  }
}