using System;
using System.Collections.Generic;

namespace DalskiAgent.Perception {
  
  /// <summary>
  ///   The Perception Unit (PU) is a container responsible for querying the attached
  ///   sensors and storing the retrieved results for further usage in the planning phase.
  /// </summary>
  public class PerceptionUnit {
    
    /* Here follows a listing of sensors and input storages. It is distinguished between
     * data sensors (they get an object from a data source) and channel-based communication
     * sensors, who listen on a channel and report all present messages to this unit, 
     * also wrapped in a SensorInput. The integer represent information type or channel. */

    private readonly List<DataSensor> _dataSensors;           // Available data sensors.
    private readonly List<CommunicationSensor> _comSensors;   // Communication sensors. 
    private readonly Dictionary<int, SensorInput> _dataInput; // Storage for data input.
    private readonly Dictionary<int, SensorInput> _messages;  // Storage for messages.


    /// <summary>
    ///   Create a perception unit (initialized with no sensors).
    /// </summary>
    public PerceptionUnit() {      
      _dataSensors = new List<DataSensor>();
      _comSensors = new List<CommunicationSensor>();
      _dataInput = new Dictionary<int, SensorInput>();
      _messages = new Dictionary<int, SensorInput>();
    }


    /// <summary>
    ///   Add a new sensor to the perception unit.
    /// </summary>
    /// <param name="sensor">The sensor to be added.</param>
    public void AddSensor(Sensor sensor) {
      if (sensor is DataSensor) _dataSensors.Add((DataSensor)sensor);
      else if (sensor is CommunicationSensor) _comSensors.Add((CommunicationSensor)sensor);
      else throw new Exception("[PerceptionUnit] Error adding sensor: The sensor type "+
                               "\""+sensor.GetType()+"\" could not be matched!");
    }


    /// <summary>
    ///   Request new sensor information from all attached sensors and store them in a map.
    /// </summary>
    public void SenseAll() {
      foreach (var sensor in _dataSensors) _dataInput[sensor.GetInformationType()] = sensor.Sense();      
      foreach (var sensor in _comSensors)  _messages [sensor.Channel] = sensor.Sense();
    }


    /// <summary>
    ///   GET-method to retrieve a piece of information.
    /// </summary>
    /// <param name="informationType">The information type to query.</param> 
    /// <returns>An input object containing the desired information.</returns>
    public SensorInput GetData(int informationType) {
      if (_dataInput.ContainsKey(informationType)) return _dataInput[informationType];
      throw new Exception("[PerceptionUnit] Error requesting input of type \""+
                          informationType+"\". No sensor enlisted for this type.");
    }


    /// <summary>
    ///   Method to get all messages on a channel.
    /// </summary>
    /// <param name="channel">The channel.</param> 
    /// <returns>An input object containing the desired information.</returns>
    public SensorInput GetMessages(int channel) {
      if (_messages.ContainsKey(channel)) return _messages[channel];
      throw new Exception("[PerceptionUnit] Error requesting messages on \""+
                          channel+"\". No sensor listens on that channel.");
    }
  }
}



/* The old way: Generic interface matching and return cast of supplied class.
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
*/