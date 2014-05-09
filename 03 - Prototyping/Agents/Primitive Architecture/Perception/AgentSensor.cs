using System.Collections.Generic;
using Primitive_Architecture.Agents;
using Primitive_Architecture.Dummies;

namespace Primitive_Architecture.Perception {
  internal class AgentSensor : Sensor {

    private readonly Environment _environment;

    public AgentSensor(Environment environment) : base(0) {
      _environment = environment;
    }

    protected override SensorInput RetrieveData() {
      var list = new List<Agent>(_environment.GetAllAgents());
      foreach (var agent in list) {
        //TODO Wenn nicht im Sichtbereich, rausschmeißen!
        list.Remove(agent);
      }

      // Aggregate all spotted agents in an input object. 
      var gsi = new GenericSensorInput(this);
      foreach (var agent in list) gsi.Values.Add(agent.Id, agent);
      return gsi;
    }
  }
}