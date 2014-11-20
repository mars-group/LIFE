using DalskiAgent.Movement;

namespace DalskiAgent.Environments {
  
  /// <summary>
  ///   This object interface abstracts from agents and other spatial objects 
  ///   for internal use. It implements the ISpatialEntity used by the ESC.
  /// </summary>
  public interface ISpatialObject {


    /// <summary>
    ///   Returns the position of the object.
    /// </summary>
    /// <returns>A position vector.</returns>
    Vector GetPosition();
  }
}
