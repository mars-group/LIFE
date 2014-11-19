using System;
using AgentTester.Wolves.Agents;
using AgentTester.Wolves.Environment;
using DalskiAgent.Execution;
using DalskiAgent.Agents;
using DalskiAgent.Auxiliary;
using DalskiAgent.Environments;
using GenericAgentArchitectureCommon.Datatypes;

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
    private static IEnvironment CreateWolvesScenario(IExecution exec, int grass, int sheeps, int wolves, bool esc) {
           
      // Create environment and agent spawner.
      var env = new Grassland (esc, new Vector(30, 20), true);
      new AgentSpawner(exec, env);

      // Create some initial agents.
      for (var i = 0; i < grass;  i ++) new Grass(exec, env);
      for (var i = 0; i < sheeps; i ++) new Sheep(exec, env);
      for (var i = 0; i < wolves; i ++) new Wolf (exec, env);      
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
      var env = CreateWolvesScenario(exec, 20, 10, 1, true);
      var view = CreateWolvesView(env);
      ConsoleView.LcRedirect = false;
      exec.Run(00, view);
      
       
      /*
      IExecution exec = new SeqExec(false);
      Grassland   env = new Grassland(true, new Vector(30, 20), true);
      
      Sheep agent1 = new Sheep(exec, env, new Vector(20, 10));
      Console.WriteLine("Pos: " + agent1.GetPosition());
      Console.WriteLine("Dir: " + (int) agent1.GetDirection().Yaw + "°, " + (int) agent1.GetDirection().Pitch + "°");
      Console.WriteLine("Dim: " + agent1.GetDimension()+"\n");

      env.MoveObject(agent1, new Vector(1,0));

      Console.WriteLine("Pos: " + agent1.GetPosition());
      Console.WriteLine("Dir: " + (int) agent1.GetDirection().Yaw + "°, " + (int) agent1.GetDirection().Pitch + "°");
      Console.WriteLine("Dim: " + agent1.GetDimension()+"\n");

      Console.ReadLine(); */
    }
  }
}
