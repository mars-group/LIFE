using DalskiAgent.Movement;
using GenericAgentArchitectureCommon.Interfaces;

namespace DalskiAgent.Environments {
  public interface IObject : ISpatialEntity {

    /// <summary>
    ///   Returns the position of the object.
    /// </summary>
    /// <returns>A position vector.</returns>
    Vector GetPosition();

  }
}
