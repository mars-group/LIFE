using System.Collections.Generic;
using GenericAgentArchitecture.Agents;

namespace GenericAgentArchitecture.Perception {

  public class CommunicationSensor : Sensor {

    private int _channel;  // The channel this sensor listens on.
    //private Dictionary<> 
    //TODO Wie macht man das mit den Nachrichten? Extraklasse? 
    // Wie werden die gespeichert und verwaltet? Zuordnung, Absender, Zeitstempel ...

    public CommunicationSensor(Agent agent, int channel) : base(agent) {
    }
    
    protected override SensorInput RetrieveData() {
      throw new System.NotImplementedException();
    }
  }
}
