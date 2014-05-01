using System.Collections.Generic;

namespace Primitive_Architecture.Perception {

  /// <summary>
  /// The Perception Unit (PU) is a container responsible for querying the attached
  /// sensors and storing the retrieved results for further usage in the planning phase. 
  /// </summary>
  class PerceptionUnit {

    private readonly List<Sensor> _sensors; // All sensors available to the agent.
    private readonly Dictionary<int, Input> _inputMemory; // R/O storage of (sensory) input.

    //TODO Idee: Schreibgeschützte Listenkopie für die Planungskomponente bereitstellen:
    //TODO Somit braucht die Planung die PU nicht kennen und man spart sich den Umweg.
    //TODO Diese Referenz kann öffentlich abrufbar sein, sie wird bei der Initialisierung 
    //TODO der Planung übergeben: ...new ReasoningSystem (pu.InputMemory);

    // Link auf RO-Collection: http://msdn.microsoft.com/en-us/library/ms132474.aspx

    /// <summary>
    /// Create a perception unit (initialized with no sensors).
    /// </summary>
    public PerceptionUnit() {
      _sensors = new List<Sensor>();
      _inputMemory = new Dictionary<int, Input>();
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
