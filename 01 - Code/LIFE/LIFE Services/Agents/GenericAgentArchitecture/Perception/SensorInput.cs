using System;

namespace DalskiAgent.Perception {

  /// <summary>
  ///   The SensorInput is a container class for raw sensory data with additional meta 
  ///   information on origin and creation time. It may also be used for communication input.
  /// </summary>
  public class SensorInput {

    public int InformationType { get; private set; } // The supplied type of information. 
    public long Cycle          { get; private set; } // Execution tick of creation.
    public DateTime Time       { get; private set; } // System time of creation. 
    public Sensor Origin       { get; private set; } // Sensor who has commited the data.
    public Object Data         { get; private set; } // The attached data object. 


    /// <summary>
    ///   Create a new sensor input.
    /// </summary>
    /// <param name="sensor">The sensor who created this object.</param>
    /// <param name="data">The input data as plain object. It has to be casted later in 
    /// the agent's reasoning phase. The programmer is responsible for a valid cast!</param>
    /// <param name="informationType">The supplied type of information (enum value).</param>
    /// <param name="cycle">Execution tick of creation.</param>
    public SensorInput(Sensor sensor, Object data, int informationType, long cycle) {
      Origin = sensor;
      InformationType = informationType;
      Cycle = cycle;
      Time = DateTime.Now;
      Data = data;
    }


    /// <summary>
    ///   Return the values of this input object as string output.  
    /// </summary>
    /// <returns>String of content.</returns>
    public new String ToString() {    
      var infType = "<unspecified>";
      if (Origin is DataSensor) infType = "Data source (type "+InformationType+")";
      else if (Origin is CommunicationSensor) infType = "Message (on "+InformationType+")";
      return "[SensorInput] Information type: " + infType + "\n" +
             "              Origin: " + Origin + "\n" +
             "              Cycle: " + Cycle + "\n" +
             "              Time: " + Time.ToString("yyyy-MM-dd HH:mm:ss") + "\n" +
             "              Data: " + Data;
    }
  }
}