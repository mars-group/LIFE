using System.Collections.Generic;
using Primitive_Architecture.Agents.Heating;
using Primitive_Architecture.Interfaces;

namespace Primitive_Architecture.Agents {

  /// <summary>
  /// This builder provides a convenient creation of agents.
  /// </summary>
  static class AgentBuilder {

    /// <summary>
    /// Create some agents related to the heating scenario.
    /// </summary>
    /// <returns>A list of agents.</returns>
    public static List<ITickClient> CreateHeatingScenarioAgents() {
      var tempEnvironment = new TempEnvironment();
      var heater = new HeaterAgent(tempEnvironment);


      return new List<ITickClient> {tempEnvironment, heater};
    }
  }
}
