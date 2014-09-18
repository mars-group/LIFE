using System.Collections.Generic;
using AgentTester.Wolves.Agents;
using ESCTestLayer.Implementation;
using LayerAPI.Interfaces;

namespace AgentTester {

  /// <summary>
  /// This builder provides a convenient creation of agents.
  /// </summary>
  static class AgentBuilder {

    /// <summary>
    /// Builder for the wolves vs. sheeps scenario.
    /// </summary>
    /// <returns>The environment.</returns>
    public static ITickClient CreateWolvesScenarioEnvironment() {

      const int grass  = 12;
      const int sheeps = 6;
      const int wolves = 2;

      var n1 = grass;
      var n2 = n1 + sheeps;
      var n3 = n2 + wolves;
      var environment = new Grassland (true) {RandomExecution = true};
      for (var i =  0; i < n1; i++) environment.AddAgent(new Grass(environment, "#"+(i<10? "0" : "")+i));
      for (var i = n1; i < n2; i++) environment.AddAgent(new Sheep(environment, "#"+(i<10? "0" : "")+i));
      for (var i = n2; i < n3; i++) environment.AddAgent(new Wolf (environment, "#"+(i<10? "0" : "")+i));     
      return environment;
    }

  }
}
