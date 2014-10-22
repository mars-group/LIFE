using System;
using AgentTester.Wolves.Agents;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Auxiliary;
using Environment = GenericAgentArchitecture.Environments.Environment;

namespace AgentTester.Wolves {
  
  /// <summary>
  ///   A local starter that uses the sequential executor. 
  ///   On this level, another starter for MARS system is planned.
  /// </summary>
  static class WolvesLocalStart {


    /// <summary>
    /// Builder for the wolves vs. sheeps scenario.
    /// </summary>
    /// <param name="grass">Number of grass agents.</param>
    /// <param name="sheeps">Number of sheeps.</param>
    /// <param name="wolves">Number of wolves.</param>
    /// <param name="esc">ESC reference (per default: null).  
    /// If not set, internal position management will be used.</param>
    /// <returns>A grassland with the agents.</returns>
    public static Environment CreateWolvesScenarioEnvironment(int grass, int sheeps, int wolves) {
      var env = new Grassland {RandomExecution = false};

      var n1 = grass;
      var n2 = n1 + sheeps;
      var n3 = n2 + wolves;
      for (var i =  0; i < n1; i++) new Grass(env.GetNewID(), env, env.GetRandomPosition());
      for (var i = n1; i < n2; i++) new Sheep(env.GetNewID(), env, env.GetRandomPosition());
      for (var i = n2; i < n3; i++) new Wolf (env.GetNewID(), env, env.GetRandomPosition());     
      return env;
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
        AgtListMin = 25, AgtListMax = 25,
        
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


    /// <summary>
    ///   Start the executor!
    /// </summary>
    public static void Main() {
      var environment = CreateWolvesScenarioEnvironment(20, 10, 5);
      var view = CreateWolvesView((Grassland) environment);
      new Executor(environment, view).Run(1000);      
    }
  }
}
