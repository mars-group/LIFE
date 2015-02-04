using System.Collections.Generic;
using SpatialAPI.Sensors;

namespace SpatialAPI.DataLayer {
  
  /// <summary>
  ///   Layer type that holds arbitrary objects in a continuous, three-dimensional space.
  /// </summary>
  /// <typeparam name="T">Layer data type.</typeparam>
  public interface IObjectCloudLayer<T> {


    /// <summary>
    ///   Creates a sensor for this layer and data type combination. 
    /// </summary>
    /// <returns>A sensor for this layer.</returns>
    ObjectCloudSensor<T> GetSensor(); 


    /// <summary>
    ///   Returns all objects in a query shape (specified by a position and an aligned geometry object).
    /// </summary>
    /// <param name="query">Query shape.</param>
    /// <returns>Set of data objects in query range.</returns>
    ICollection<T> Query(ShapeQuery query);
  }
}
