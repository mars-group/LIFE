using System;
using SpatialCommon.Interfaces;

namespace GenericAgentArchitectureCommon.Interfaces {
  
  public interface IDataSource {    
    
    /// <summary>
    ///   Get information of a given type from a data source.
    /// </summary>
    /// <param name="spec">Information object describing which data to query.</param>
    /// <returns>An object containing the asked information. Obviously, it has to be casted.</returns>
    Object GetData(ISpecification spec);
  }
}