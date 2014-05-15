using System;
using System.Collections.Generic;
using Common.Interfaces;

namespace Primitive_Architecture.Perception {
  
  /// <summary>
  ///   The Perception Unit (PU) is a container responsible for querying the attached
  ///   sensors and storing the retrieved results for further usage in the planning phase.
  /// </summary>
  internal class PerceptionUnit : IPerception {
    
    private readonly List<Sensor> _sensors; // All sensors available to the agent.
    private readonly Dictionary<Type, Input> _inputMemory; // Storage of sensed input.


    /// <summary>
    ///   Create a perception unit (initialized with no sensors).
    /// </summary>
    public PerceptionUnit() {
      _sensors = new List<Sensor>();
      _inputMemory = new Dictionary<Type, Input>();
    }


    /// <summary>
    ///   Add a new sensor to the perception unit.
    /// </summary>
    /// <param name="sensor">The sensor to be added.</param>
    public void AddSensor(Sensor sensor) {
      _sensors.Add(sensor);
    }


    /// <summary>
    ///   Request new sensor information from all attached sensors and store them in a map.
    /// </summary>
    public void SenseAll() {
      foreach (var sensor in _sensors) {
        var input = sensor.Sense();
        _inputMemory[input.GetType()] = input;
      }
    }


    /// <summary>
    ///   GET-method to retrieve a piece of information.
    /// </summary>
    /// <returns>An input object containing the desired information.</returns>
    public T GetData<T>() where T : class {
      Input value;
      var success = _inputMemory.TryGetValue(typeof (T), out value);
      if (success) return (T) value; 
   
      // Retry with interface matching.
      foreach (var input in _inputMemory.Values) {
        if (input is T) return (T) input;      
      }
      return null;
    }
  }
}