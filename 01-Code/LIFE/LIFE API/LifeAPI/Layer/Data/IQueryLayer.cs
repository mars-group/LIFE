using System.Linq;
using LifeAPI.Perception.Sensors;

namespace LifeAPI.Layer.Data {

  /// <summary>
  ///   Data layer capable of LINQ query evaluation.
  /// </summary>
  /// <typeparam name="T">Layer data type.</typeparam>
  public interface IQueryLayer<T> {


    /// <summary>
    ///   Creates a sensor for this layer and data type combination. 
    /// </summary>
    /// <returns>A sensor for this layer.</returns>
    QuerySensor<T> GetSensor();


    /// <summary>
    ///   Returns a set of layer data that matches the query.
    /// </summary>
    /// <param name="query">LINQ query.</param>
    /// <returns>Layer data subset.</returns>
    IQueryable<T> GetQuery(string query);
  }
}
