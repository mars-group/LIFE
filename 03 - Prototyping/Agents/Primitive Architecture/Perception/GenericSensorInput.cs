using System.Collections.Generic;

namespace Primitive_Architecture.Perception {

  /// <summary>
  /// The Generic Sensor Input (GSI) is merely a mapping structure to store arbitrary data. 
  /// It is based on the Input/SensorInput hierarchy to provide additional information.  
  /// </summary>
  internal class GenericSensorInput : SensorInput {

    public Dictionary<object, object> Values { get; private set; } 

    /// <summary>
    /// Create a new generic sensor input object. 
    /// 
    /// </summary>
    /// <param name="originSensor">The sensor that created this input object.</param>
    public GenericSensorInput(Sensor originSensor) : base(originSensor) {
      Values = new Dictionary<object, object>();
    }

  }
}