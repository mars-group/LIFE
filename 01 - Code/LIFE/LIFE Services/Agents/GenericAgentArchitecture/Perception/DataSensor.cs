using DalskiAgent.Agents;
using GenericAgentArchitectureCommon.Interfaces;

namespace DalskiAgent.Perception {
  
  /// <summary>
  ///   A universal sensor, suitable for primitive information retrieval. 
  ///   It may be considered as some kind of middle layer to pass input along.
  /// </summary>
  public class DataSensor : Sensor {

    protected readonly IGenericDataSource Source;    // The data source (environment) to sense. 
    public int InformationType { get; private set; } // Link to the data type to acquire.
    protected readonly IHalo Halo;                   // The area this sensor can percept. 


    /// <summary>
    ///   Create a simple sensor. It retrieves sensor information to a given type
    ///   from an environment that offers "SensorInput" without further processing.
    /// </summary>
    /// <param name="agent">The agent who owns this sensor.</param>
    /// <param name="source">Reference to the queried data source.</param>
    /// <param name="dataType">The information type, this sensor listens at.</param>
    /// <param name="halo">The halo represents the perception field for this sensor.</param>
    public DataSensor(Agent agent, IGenericDataSource source, int dataType, IHalo halo) 
      : base (agent) {
      Source = source;
      InformationType = dataType;
      Halo = halo;
    }


    /// <summary>
    ///   This method shall provide an implementation for active information retrieval.
    ///   In passive mode, the input object is directly set by the source and hence no 
    ///   polling is performed here.
    /// </summary>
    /// <returns>Sensor data object.</returns>
    protected override SensorInput RetrieveData() {
      var result = Source.GetData(InformationType, Halo);
      return new SensorInput(this, result, InformationType, Agent.GetTick());
    }


    /// <summary>
    ///   Switch this sensor between active (polling) and reactive (callback) mode.
    /// </summary>
    /// <param name="active">Set this sensor active (true) or passive (false).</param>
    /// <returns>Return value tells, whether this operation succeeded or not.</returns>
    public bool SetActive(bool active) {
      if (!(Source is ICallbackDataSource)) return false;
      if (Active != active) {
        (Source as ICallbackDataSource).SetCallbackMode(!active, LastInput);
        Active = active;
      }
      return true;
    }
  }
}