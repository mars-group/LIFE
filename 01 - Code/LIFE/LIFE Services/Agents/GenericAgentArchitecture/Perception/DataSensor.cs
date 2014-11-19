using System;
using DalskiAgent.Agents;
using GenericAgentArchitectureCommon.Interfaces;

namespace DalskiAgent.Perception {
  
  /// <summary>
  ///   A universal sensor, suitable for primitive information retrieval. 
  ///   It may be considered as some kind of middle layer to pass input along.
  /// </summary>
  public class DataSensor : Sensor {

    private readonly IGenericDataSource _source;      // The data source (environment) to sense. 
    private readonly ISpecificator _specificator;     // The area this sensor can percept. 
    public Enum InformationType { get; private set; } // This sensor's information type.


    /// <summary>
    ///   Create a simple sensor. It retrieves sensor information to a given type
    ///   from an environment that offers "SensorInput" without further processing.
    /// </summary>
    /// <param name="agent">The agent who owns this sensor.</param>
    /// <param name="source">Reference to the queried data source.</param>
    /// <param name="specificator">The specificator represents the perception field for this sensor.</param>
    public DataSensor(Agent agent, IGenericDataSource source, ISpecificator specificator) 
      : base (agent) {
      _source = source;
      _specificator = specificator;
      InformationType = specificator.GetInformationType();
    }


    /// <summary>
    ///   This method shall provide an implementation for active information retrieval.
    ///   In passive mode, the input object is directly set by the source and hence no 
    ///   polling is performed here.
    /// </summary>
    /// <returns>Sensor data object.</returns>
    protected override SensorInput RetrieveData() {
      var result = _source.GetData(_specificator);
      return new SensorInput(this, result,  Convert.ToInt32(InformationType), Agent.GetTick());
    }


    /// <summary>
    ///   Switch this sensor between active (polling) and reactive (callback) mode.
    /// </summary>
    /// <param name="active">Set this sensor active (true) or passive (false).</param>
    /// <returns>Return value tells, whether this operation succeeded or not.</returns>
    public bool SetActive(bool active) {
      if (!(_source is ICallbackDataSource)) return false;
      if (Active != active) {
        (_source as ICallbackDataSource).SetCallbackMode(!active, LastInput);
        Active = active;
      }
      return true;
    }
  }
}