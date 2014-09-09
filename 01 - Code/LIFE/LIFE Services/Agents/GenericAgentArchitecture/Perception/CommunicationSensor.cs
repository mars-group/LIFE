using GenericAgentArchitecture.Agents;

namespace GenericAgentArchitecture.Perception {

  public class CommunicationSensor : Sensor {

    public int Channel { get; private set; }  // The channel this sensor listens on.
    //private Dictionary<> 
    //TODO Wie macht man das mit den Nachrichten? Extraklasse? 
    //TODO Wie werden die gespeichert und verwaltet? Zuordnung, Absender, Zeitstempel ...

    public CommunicationSensor(Agent agent, int channel) : base(agent) {}
    
    protected override SensorInput RetrieveData() {
      throw new System.NotImplementedException();
    }
  }
}

/*
Messages are only for one tick audible - after that, they will fade away. So the agent won't
notice any messages when it does not listen (sensor off) neither will it see messages prior 
to listening start.
*/