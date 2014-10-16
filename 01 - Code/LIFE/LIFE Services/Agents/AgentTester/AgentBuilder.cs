using System;
using AgentTester.Wolves.Agents;
using ESCTestLayer.Interface;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Auxiliary;
using GenericAgentArchitecture.Environments;
using LayerAPI.Interfaces;

namespace AgentTester {

  /// <summary>
  /// This builder provides a convenient creation of agents.
  /// </summary>
  internal static class AgentBuilder {

    /// <summary>
    /// Builder for the wolves vs. sheeps scenario.
    /// </summary>
    /// <param name="grass">Number of grass agents.</param>
    /// <param name="sheeps">Number of sheeps.</param>
    /// <param name="wolves">Number of wolves.</param>
    /// <param name="esc">ESC reference (per default: null).  
    /// If not set, internal position management will be used.</param>
    /// <returns>A grassland with the agents.</returns>
    public static ITickClient CreateWolvesScenarioEnvironment(int grass, int sheeps, int wolves, IESC esc = null) {

      var grassland = new Grassland {RandomExecution = false};
      IEnvironment env;

      // If ESC exists, create adapter and use it as position manager. Otherwise use internal.
      if (esc != null) {
        var adapter = new ESCAdapter(esc);
        env = adapter;
      }
      else env = grassland;

      var n1 = grass;
      var n2 = n1 + sheeps;
      var n3 = n2 + wolves;
      for (var i =  0; i < n1; i++) new Grass(grassland.GetNewID(), env, grassland.GetRandomPosition());
      for (var i = n1; i < n2; i++) new Sheep(grassland.GetNewID(), env, grassland.GetRandomPosition());
      for (var i = n2; i < n3; i++) new Wolf (grassland.GetNewID(), env, grassland.GetRandomPosition());
      
      return grassland;
    }


    /// <summary>
    ///   Create a console view for the wolves vs. sheeps scenario.
    /// </summary>
    /// <param name="env">The grassland reference.</param>
    /// <returns>The console view.</returns>
    public static ConsoleView CreateWolvesView(Grassland env) {
      return new ConsoleView(new ConsoleInitData{

        // Header strings, map size and options. 
        Scenario = "Wolfszenario", MapX = 30, MapY = 18, MessageLines = 5,
        AgentsHeader = new [] {" ID ","  Typ  "," Position "," Energie "," Hgr."," G/S/W "," Distanz ", " Regel "},
        AgtListMax = 25,
        
        // Agent color definitions.
        GetColor = delegate(SpatialAgent agt) {
          if (agt is Grass) return ConsoleColor.Green;
          if (agt is Wolf)  return ConsoleColor.Red;
          if (agt is Sheep) return ConsoleColor.Blue;
          return ConsoleColor.Gray;
        },
        
        // Agent symbol definitions.
        GetSymbol = delegate(SpatialAgent agt) {
          if (agt is Grass) {
            if (((Grass) agt).GetFoodValue() > 40) return '▓';
            if (((Grass) agt).GetFoodValue() > 20) return '▒';
                                                   return '░';
          }
          if (agt is Wolf)  return 'W';
          if (agt is Sheep) return 'S';
          return '█';
        },
      }, env);
    }
  }
}