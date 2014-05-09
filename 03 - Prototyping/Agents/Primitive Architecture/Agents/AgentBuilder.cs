using Primitive_Architecture.Agents.Heating;
using Primitive_Architecture.Agents.Ice;
using Primitive_Architecture.Agents.Wolves;
using Primitive_Architecture.Dummies;

namespace Primitive_Architecture.Agents {

  /// <summary>
  /// This builder provides a convenient creation of agents.
  /// </summary>
  static class AgentBuilder {


    /// <summary>
    /// Create an environment and some agents related to the heating scenario.
    /// </summary>
    /// <returns>The environment container.</returns>
    public static Environment CreateHeatingScenarioEnvironment() {
      var tempEnvironment = new TempEnvironment();
      var heater = new HeaterAgent(tempEnvironment);
      var controller = new TempAgent(tempEnvironment, heater);
      var smith = new AgentSmith(tempEnvironment);

      tempEnvironment.AddAgent(heater);
      tempEnvironment.AddAgent(controller);
      tempEnvironment.AddAgent(smith);
      return tempEnvironment;
    }


    /// <summary>
    /// Builder for the wolves vs. sheeps scenario.
    /// </summary>
    /// <returns>The environment.</returns>
    public static Environment CreateWolvesScenarioEnvironment() {
      var environment = new Grassland();
      environment.RandomExecution = true;
      for (var i = 0; i < 8; i++) environment.AddAgent(new Grass("[Grass " +i+"]"));
      for (var i = 0; i < 4; i++) environment.AddAgent(new Sheep("[Sheep " +i+"]"));
      for (var i = 0; i < 2; i++) environment.AddAgent(new Wolf ("[Wolf  " +i+"]"));     
      return environment;
    }


    /// <summary>
    /// Builder for the iceworld scenario.
    /// </summary>
    /// <returns>The environment.</returns>
    public static Environment CreateIceworldScenarioEnvironment() {
      var environment = new IceWorld();
      environment.AddAgent(new Iceeater(environment));
      environment.AddAgent(new Iceman(environment));
      environment.RandomExecution = true;
      return environment;
    }
  }
}
