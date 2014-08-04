using GenericAgentArchitecture.Dummies;

namespace GenericAgentArchitecture.Perception {
  
  /// <summary>
  ///   The agent sensor senses other agents.
  /// </summary>
  public class AgentSensor : Sensor {

    private readonly Environment _environment;  // The data source to acquire agents from.


    /// <summary>
    ///   Create a sensor that percepts nearby agents.
    /// </summary>
    /// <param name="environment">The data source to acquire agents from.</param>
    /// <param name="halo">The halo to use. May be left blank for default omni halo.</param>
    public AgentSensor(Environment environment, Halo halo = null) : base(0) {
      _environment = environment;
      if (halo != null) Halo = halo;
    }


    /// <summary>
    ///   Combine total agent set and halo perception. 
    /// </summary>
    /// <returns>A generic list with agent distances and references.</returns>
    protected override SensorInput RetrieveData() {
      var gsi = new GenericSensorInput(this);
      foreach (var agent in _environment.GetAllAgents()) {
        if (Halo.IsInRange(agent.Position) && agent.Position != Halo.Position) {
          gsi.Values.Add(agent.Id, agent);
        }
      }
      return gsi;
    }
  }
}