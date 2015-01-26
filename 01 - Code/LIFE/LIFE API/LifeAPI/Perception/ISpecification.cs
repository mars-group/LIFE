using System;

namespace LifeAPI.Perception {
  
  /// <summary>
  ///   Information object describing which data to query.
  /// </summary>
  public interface ISpecification {

      /// <summary>
      ///   Return the information type specified by this object.
      /// </summary>
      /// <returns>Information type (as enum value).</returns>
      Enum InformationType { get; }
  }
}