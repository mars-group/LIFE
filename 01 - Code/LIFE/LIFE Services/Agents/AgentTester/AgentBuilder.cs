using AgentTester.Wolves.Agents;
using LayerAPI.Interfaces;

namespace AgentTester {

  /// <summary>
  /// This builder provides a convenient creation of agents.
  /// </summary>
  static class AgentBuilder {
    
    
    /// <summary>
    /// Builder for the wolves vs. sheeps scenario.
    /// </summary>
    /// <param name="grass">Number of grass agents.</param>
    /// <param name="sheeps">Number of sheeps.</param>
    /// <param name="wolves">Number of wolves.</param>
    /// <returns>A grassland with the agents.</returns>
    public static ITickClient CreateWolvesScenarioEnvironment(int grass, int sheeps, int wolves) {
      var n1 = grass;
      var n2 = n1 + sheeps;
      var n3 = n2 + wolves;
      var env = new Grassland {RandomExecution = true};
      for (var i =  0; i < n1; i++) new Grass(env.GetNewID(), env, env.GetRandomPosition());
      for (var i = n1; i < n2; i++) new Sheep(env.GetNewID(), env, env.GetRandomPosition());
      for (var i = n2; i < n3; i++) new Wolf (env.GetNewID(), env, env.GetRandomPosition());     
      return env;
    }
  }
}
