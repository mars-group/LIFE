using System;
using AgentTester.Wolves.Agents;
using DalskiAgent.Execution;
using DalskiAgent.Movement;
using ESCTestLayer.Implementation;
using DalskiAgent.Agents;
using DalskiAgent.Auxiliary;
using DalskiAgent.Environments;

namespace AgentTester.Wolves {
  
  /// <summary>
  ///   A local starter that uses the sequential executor. 
  ///   On this level, another starter for MARS system is planned.
  /// </summary>
  static class WolvesLocalStart {
      
    /// <summary>
    /// Builder for the wolves vs. sheeps scenario.
    /// </summary>
    /// <param name="exec">Sequential execution class.</param>
    /// <param name="grass">Number of grass agents.</param>
    /// <param name="sheeps">Number of sheeps.</param>
    /// <param name="wolves">Number of wolves.</param>
    /// <param name="esc">'True': ESC instance created. 
    /// 'False': Own environment with collision prevention.</param>
    /// <returns>A grassland with the agents.</returns>
    private static IEnvironment CreateWolvesScenario(SeqExec exec, int grass, int sheeps, int wolves, bool esc) {
      
      IEnvironment env;
      if (!esc) env = new Grassland (exec);
      else env = new ESCAdapter(new UnboundESC(), new Vector(30, 20), true);
      
      var n1 = grass;
      var n2 = n1 + sheeps;
      var n3 = n2 + wolves;
      
      for (var i =  0; i < n1; i++) new Grass(exec, env);
      for (var i = n1; i < n2; i++) new Sheep(exec, env);
      for (var i = n2; i < n3; i++) new Wolf (exec, env);      
      return env;
    }


    /// <summary>
    ///   Create a console view for the wolves vs. sheeps scenario.
    /// </summary>
    /// <param name="env">The grassland reference.</param>
    /// <returns>The console view.</returns>
    private static ConsoleView CreateWolvesView(IEnvironment env) {
      return new ConsoleView(new ConsoleInitData{

        // Header strings, map size and options. 
        Scenario = "Wolfszenario", MapX = 30, MapY = 20, MessageLines = 5,
        AgentsHeader = new [] {" ID ","  Typ  "," Position "," Energie "," Hgr."," G/S/W "," Distanz ", " Regel "},
        AgtListMin = 27, AgtListMax = 27,
        
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
      var exec = new SeqExec(true);
      var env = CreateWolvesScenario(exec, 18, 6, 2, false);
      var view = CreateWolvesView(env);
      ConsoleView.LcRedirect = false;
      exec.Run(750, view);
    }
  }
}
