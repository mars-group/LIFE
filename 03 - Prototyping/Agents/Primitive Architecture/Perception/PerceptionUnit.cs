using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Primitive_Architecture.Perception {

  /// <summary>
  /// The Perception Unit (PU) is a container responsible for querying the attached
  /// sensors and storing the retrieved results for further usage in the planning phase. 
  /// </summary>
  class PerceptionUnit {

    private readonly List<Sensor> _sensors; // All sensors available to the agent.
    private readonly Dictionary<int, Input> _inputMemory; // Storage of (sensory) input.
    public IReadOnlyDictionary<int, Input> InputMemory;   // Read-only version of the memory. 


    /// <summary>
    /// Create a perception unit (initialized with no sensors).
    /// </summary>
    public PerceptionUnit() {
      _sensors = new List<Sensor>();
      _inputMemory = new Dictionary<int, Input>();
      InputMemory = new ReadOnlyDictionary<int, Input>(_inputMemory);
    }


    /// <summary>
    /// Add a new sensor to the perception unit.
    /// </summary>
    /// <param name="sensor">The sensor to be added.</param>
    public void AddSensor(Sensor sensor) {
      _sensors.Add(sensor);
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
