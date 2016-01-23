using System.Linq;
using LifeAPI.Layer.Data;

namespace LifeAPI.Perception.Sensors {
  
  /// <summary>
  ///   Sensor for query data layers.
  /// </summary>  
  /// <typeparam name="T">Layer data type.</typeparam>
  public class QuerySensor<T> {

    private IQueryLayer<T> _layer;  // Layer reference.


    /// <summary>
    ///   Create a sensor for LINQ query grids.
    /// </summary>
    /// <param name="layer">The data layer.</param>
    public QuerySensor(IQueryLayer<T> layer) {
      _layer = layer;
    } 


    /// <summary>
    ///   Allows arbitrary queries with the LINQ interface.
    /// </summary>
    /// <param name="query">LINQ query.</param>
    /// <returns>Layer data subset.</returns>
    public IQueryable<T> GetQuery(string query) {
      return _layer.GetQuery(query);
    }
  }
}
