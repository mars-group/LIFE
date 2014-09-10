using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Dummies;

namespace GenericAgentArchitecture.Perception {

  /// <summary>
  ///   This sensor listens on a channel and returns all messages to the PU. 
  /// </summary>
  public class CommunicationSensor : Sensor {

    public int Channel { get; private set; }  // The channel this sensor listens on.
    
    /// <summary>
    ///   Create a communication sensor.
    /// </summary>
    /// <param name="agent">The agent who owns this sensor.</param>
    /// <param name="channel">The channel this sensor listens on.</param>
    public CommunicationSensor(Agent agent, int channel) : base(agent) {
      Channel = channel;
    }
    

    /// <summary>
    ///   Returns all new messages the the perception unit. 
    /// </summary>
    /// <returns>Sensor input object with list of new messages.</returns>
    protected override SensorInput RetrieveData() {
      return new SensorInput(this, MessageServer.GetMessages(Channel), Channel, Agent.Cycle);
    }
  }
}
