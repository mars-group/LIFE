using Primitive_Architecture.Interfaces;

namespace Primitive_Architecture.Perception {
  
  /// <summary>
  ///   A universal sensor, suitable for primitive information retrieval. 
  ///   It may be considered as some kind of middle layer to pass input along.
  /// </summary>
  internal class GenericSensor : Sensor {

    /// <summary>
    ///   Create a simple sensor. It retrieves sensor information to a given type
    ///   from an environment that offers "SensorInput" without further processing.
    /// </summary>
    /// <param name="source">Reference to the queried data source.</param>
    /// <param name="dataType">The information type, this sensor listens at.</param>
    public GenericSensor(IGenericDataSource source, int dataType) : base (dataType) {
      Source = source;
    }


    /// <summary>
    ///   Retrieve input data from the generic environment interface.
    ///   The generic aspect does not allow any further processing.
    /// </summary>
    /// <returns>Sensor object containing the requested data type.</returns>
    protected override SensorInput RetrieveData() {
      return Source.GetData(InformationType);
    }
  }
}