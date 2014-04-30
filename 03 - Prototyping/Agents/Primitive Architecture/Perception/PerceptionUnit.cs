using System.Collections.Generic;

namespace Primitive_Architecture.Perception {

  /// <summary>
  /// The Perception Unit (PU) is a container responsible for querying the attached
  /// sensors and storing the retrieved results for further usage in the planning phase. 
  /// </summary>
  class PerceptionUnit {

    private readonly List<Sensor> _sensors; // All sensors available to the agent.
    private readonly Dictionary<int, Input> _inputMemory; // R/O storage of (sensory) input.

    /// <summary>
    /// Create a perception unit gives some sensors.
    /// </summary>
    /// <param name="sensors">A list of all attached sensors.</param>
    public PerceptionUnit(List<Sensor> sensors) {
      _sensors = sensors;
      _inputMemory = new Dictionary<int, Input>();
    }


    /// <summary>
    /// Request new sensor information from all attached sensors and store them in a map.
    /// </summary>
    public void SenseAll() {
      foreach (var sensor in _sensors) {
        _inputMemory [sensor.InformationType] = sensor.Sense();
      }  
    }


    /// <summary>
    /// GET-method to retrieve a piece of information. 
    /// </summary>
    /// <param name="inputType">The information type to query, specified by an enum.</param>
    /// <returns>An input object containing the desired information.</returns>
    public Input GetData(int inputType) {
      return _inputMemory[inputType];
    }

  }
}
