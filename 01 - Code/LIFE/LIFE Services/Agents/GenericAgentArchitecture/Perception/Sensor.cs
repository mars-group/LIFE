using DalskiAgent.Agents;

namespace DalskiAgent.Perception {
  
  /// <summary>
  ///   This sensor class provides an abstract base for custom implementations.
  /// </summary>
  public abstract class Sensor {
     
    public bool Enabled { get; set; }  // Shows if sensor is operational or not.
    protected readonly Agent Agent;    // The agent who owns this sensor.
    protected bool Active;             // This boolean controls polling or callback mode.   
    protected SensorInput LastInput;   // Container for the last retrieved sensor input.  
   

    /// <summary>
    ///   Create an abstract sensor, serving as a base for either a type-specific
    ///   sensor or a generic sensor that acquires a given type of information.
    /// </summary>
    protected Sensor(Agent agent) {
      Agent = agent;
      Enabled = true;
      Active = true;
      LastInput = null;
    }


    /// <summary>
    ///   Retrieve the sensor data.
    /// </summary>
    /// <returns>The latest sensor information.</returns>
    public SensorInput Sense() {
      if (!Enabled) return null;
      if (Active) LastInput = RetrieveData();
      return LastInput;
    }


    /// <summary>
    ///   This method shall provide an implementation for active information retrieval.
    /// </summary>
    /// <returns>An information object, acquired via some polling function.</returns>
    protected abstract SensorInput RetrieveData();
  }
}