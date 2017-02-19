using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LIFE.Components.DalskiAgent.Perception {

  /// <summary>
  ///   Simple sensor interface. Just requests a sensing method.
  /// </summary>
  public interface ISensor {

    /// <summary>
    ///   Performs data gathering.
    /// </summary>
    /// <returns>Sensor return value wrapped as "object".</returns>
    object Sense();
  }


  /// <summary>
  ///   Sensor aggregation unit and data storage.
  ///   It represents the agent's "eyes and ears".  
  /// </summary>
  public class SensorArray {
   
    private readonly Dictionary<ISensor, string> _sensors;           // Sensor references for query.
    private readonly ConcurrentDictionary<string, object> _results;  // Return value mapping. 


    /// <summary>
    ///   Create a new sensor array.
    /// </summary>
    public SensorArray() {
      _sensors = new Dictionary<ISensor, string>();
      _results = new ConcurrentDictionary<string, object>();
    }


    /// <summary>
    ///   Adds a sensor to the agent. 
    ///   This method should be used in the constructor of the specific agent.
    /// </summary>
    /// <param name="sensor">The sensor to add.</param>
    /// <param name="name">Sensor identifier.</param>
    public void AddSensor(ISensor sensor, string name = "") {
      if (name.Equals("")) name = sensor.GetType().ToString();
      _sensors.Add(sensor, name);
      _results[name] = null;
    }


    /// <summary>
    ///   Removes a sensor by its name. 
    /// </summary>
    /// <param name="name">Sensor identifier.</param>
    public void DeleteSensor(string name) {
      object obj;
      if (_results.ContainsKey(name)) _results.TryRemove(name, out obj);
      ISensor s = null;
      foreach (var sensor in _sensors.Keys) {
        if (_sensors[sensor].Equals(name)) {
          s = sensor;
          break;
        }
      }
      if (s != null) _sensors.Remove(s);
    }


    /// <summary>
    ///   Removes a sensor.
    /// </summary>
    /// <param name="sensorType">Type of sensor to delete. If multiple sensors exist, all are deleted.</param>
    public void DeleteSensor(Type sensorType) {
      DeleteSensor(sensorType.ToString());
    }



    /// <summary>
    ///   Loops over all sensors and updates their data fields.
    /// </summary>
    public void SenseAll() {
      Parallel.ForEach(_sensors.Keys, sensor => {
        var result = sensor.Sense();
        var id = _sensors[sensor];
        _results[id] = result;
      });
    }


    /// <summary>
    ///   Returns a sensor's data value (access by sensor type). 
    /// </summary>
    /// <typeparam name="TSensor">The queried sensor type.</typeparam>
    /// <typeparam name="TResult">The matching return value.</typeparam>
    /// <returns>Either the retrieved data or an exception (query/cast).</returns>
    public TResult Get<TSensor, TResult>() {
      var sensorType = typeof (TSensor);
      return Get<TResult>(sensorType.ToString());
    } 


    /// <summary>
    ///   Returns a sensor's data value (access by sensor ID). 
    /// </summary>
    /// <typeparam name="TResult">The matching return value.</typeparam>
    /// <param name="sensorId">Sensor identifier.</param>
    /// <returns>Either the retrieved data or an exception (query/cast).</returns>
    public TResult Get<TResult>(string sensorId) {
      if (_results.ContainsKey(sensorId)) {
        var obj = _results[sensorId];
        if (obj == null) return default (TResult);  // Return 'null' to prevent cast exceptions.
        if (obj is TResult) return (TResult) (obj);
        throw new SensorReturnsOtherTypeException("[SensorArray] Cast error: '"+sensorId+"' does not return "+typeof(TResult)+"!");
      }
      throw new SensorNotFoundException("[SensorArray] No value for sensor '"+sensorId+"' listed!");
    } 



    /// <summary>
    ///   [DEBUG] Write all sensors and their current values to the console.
    /// </summary>
    public void DebugSensors() {
      foreach (var sensor in _sensors.Keys) {
        Console.WriteLine("<"+sensor.GetType()+">: '"+_sensors[sensor]+"' ==> "+_results[_sensors[sensor]]);
      }
    }
  }



  /// <summary>
  ///   Exception to be thrown when a wrong value type for a sensor is queried.
  /// </summary>
  [Serializable]
  public class SensorReturnsOtherTypeException : Exception {

    /// <summary>
    ///   Create new exception.
    /// </summary>
    /// <param name="msg">Detailed exception message.</param>
    public SensorReturnsOtherTypeException (string msg) : base(msg) {}
  }


  /// <summary>
  ///   A not-existant sensor was queried.
  /// </summary>
  [Serializable]
  public class SensorNotFoundException : Exception {

    /// <summary>
    ///   Create new exception.
    /// </summary>
    /// <param name="msg">Detailed exception message.</param>
    public SensorNotFoundException (string msg) : base(msg) {}
  }
}