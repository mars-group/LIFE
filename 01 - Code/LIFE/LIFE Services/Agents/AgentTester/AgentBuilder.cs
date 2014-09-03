﻿using System.Collections.Generic;
using AgentTester.RandomMove_ESC;
using AgentTester.Wolves.Agents;
using ESCTestLayer;
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


    /// <summary>
    /// Builder for random walk agents layer.
    /// </summary>
    public static ITickClient CreateRandomMovingAgents(int nr, int dimX, int dimY) {        
      var esc = new ESC();
      var agents = new List<ITickClient>();
      for (var i = 0; i < nr; i++) agents.Add(new WalkerAgent("WA-"+i, esc));      
      var layer = new WalkerLayer(agents);
      //layer.InitLayer<Object>(null, null, null);
      return layer;
    }
  }
}
