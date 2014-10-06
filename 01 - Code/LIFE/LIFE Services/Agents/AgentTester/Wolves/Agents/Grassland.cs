using System;
using System.Collections.Generic;
using System.Linq;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Movement;
using LayerAPI.Interfaces;
using TVector = CommonTypes.DataTypes.Vector;

namespace AgentTester.Wolves.Agents {

  /// <summary>
  ///   This grassland is home to sheeps and wolves ... and yes, 'grass'.
  /// </summary>
  internal class Grassland : Environment2D, IGenericDataSource {
    
    private int _idCounter;   // This counter is needed for agent ID distribution.

    /// <summary>
    ///   Create a new grassland.
    /// </summary>
    public Grassland() : base (new Vector(30, 18)) {
      _idCounter = 0;
      PrintInformation = true;

      // Configure console window for debug information.     
      Console.SetWindowSize(100,  32);
      Console.SetBufferSize(100, 200);       
    }


    /// <summary>
    ///   In this simple scenario, there is no need for environmental evolution. 
    ///   Nevertheless, the spawning of some additional grass agents would be nice.
    /// </summary>
    protected override void AdvanceEnvironment() {
      var grassCount = Agents.OfType<Grass>().Count();
      if (Random.Next(40+grassCount) < 20) {
        new Grass(_idCounter, this, GetRandomPosition());
        _idCounter++;
      }
    }


    /// <summary>
    ///   Console output function. Prints the 2D environment and all agent logs.
    /// </summary>
    protected override void PrintEnvironment() {
      Console.Clear();

      // Output scenario name and current cycle.
      Console.SetCursorPosition(0, 0);
      Console.ForegroundColor = ConsoleColor.Red;
      Console.Write(" » WOLFSZENARIO");
      Console.ForegroundColor = ConsoleColor.Gray;
      Console.SetCursorPosition(0, 1);
      Console.Write("   Zyklus: "+Cycle);          

      // Paint the environmental borders.   // 191, 192, |179, -196, 217, 218
      var str = "┌";
      for (var i = 0; i < Boundaries.X; i ++) str += "─";
      str += "┐\n";
      for (var y = 0; y < Boundaries.Y; y ++) {
        str += "│";
        for (var x = 0; x < Boundaries.X; x ++) str += " ";
        str += "│\n";
      }
      str += "└";
      for (var i = 0; i < Boundaries.X; i ++) str += "─";
      str += "┘";     
      Console.SetCursorPosition(0, 2);
      Console.WriteLine(str);

      // Print the agents.
      foreach (var agent in Agents) {
        var pos = ((SpatialAgent) agent).GetPosition();
        Console.SetCursorPosition((int)pos.X+1, (int)pos.Y+3);
        if (agent is Wolf) {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.Write("W");
        }
        else if (agent is Sheep) {
          Console.ForegroundColor = ConsoleColor.Blue;
          Console.Write("S");
        }
        else if (agent is Grass) {
          var g = (Grass) agent;
          Console.ForegroundColor = ConsoleColor.Green;
          if      (((double)g.GetFoodValue() / Grass.FoodvalueMax) > 0.66) Console.Write("▓");
          else if (((double)g.GetFoodValue() / Grass.FoodvalueMax) > 0.33) Console.Write("▒");
          else                                                             Console.Write("░");      
        }
      }
      Console.ForegroundColor = ConsoleColor.Gray;

      // Print the individual agent debug messages.
      var xo = (int) Boundaries.X+4;
      Console.SetCursorPosition(xo, 0); Console.Write(" ID |  Typ  | Position | Energie | Hgr.| G/S/W | Distanz | Regel");            
      Console.SetCursorPosition(xo, 1); Console.Write("────┼───────┼──────────┼─────────┼─────┼───────┼─────────┼───────");
      var printed = 2;

      foreach (var agent in Agents.OfType<Grass>()) {       
        Console.SetCursorPosition(xo, printed);
        Console.Write(agent.ToString()); 
        printed ++;
      }
      if (Agents.OfType<Grass>().Any() && (Agents.OfType<Sheep>().Any() || Agents.OfType<Wolf>().Any())) {
        Console.SetCursorPosition(xo, printed);
        Console.Write("────┼───────┼──────────┼─────────┼─────┼───────┼─────────┼───────");
        printed ++;
      }
     

      foreach (var agent in Agents.OfType<Sheep>()) {
        Console.SetCursorPosition(xo, printed);
        Console.Write(agent.ToString()); 
        printed ++; 
      }
      if (Agents.OfType<Sheep>().Any() && Agents.OfType<Wolf>().Any()) {
        Console.SetCursorPosition(xo, printed);
        Console.Write("────┼───────┼──────────┼─────────┼─────┼───────┼─────────┼───────");
        printed ++;
      }


      foreach (var agent in Agents.OfType<Wolf>()) {
        Console.SetCursorPosition(xo, printed);
        Console.Write(agent.ToString()); 
        printed ++;  
      }


      // Output the total agent counts.
      Console.SetCursorPosition(0, (int)Boundaries.Y+4);
      Console.Write(" Agenten-Gesamtanzahlen:\n"+
                    "  - Gras  : "+Agents.OfType<Grass>().Count()+"\n"+
                    "  - Schafe: "+Agents.OfType<Sheep>().Count()+"\n"+
                    "  - Wölfe : "+Agents.OfType<Wolf>().Count());
      
      Console.SetCursorPosition(79, 0);
    }



    /* Data source functions: Information types and retrieval method. */

    public enum InformationTypes { Agents }
      
    public object GetData(int informationType, IGeometry geometry) {
      switch ((InformationTypes) informationType) {      
        case InformationTypes.Agents: {
          var map = new Dictionary<long, SpatialAgent>();
          foreach (var agent in GetAllAgents()) {
            var agt = (SpatialAgent) agent;
            var agtPos = new TVector(agt.GetPosition().X, agt.GetPosition().Y);
            var geoPos = new Vector(geometry.GetPosition().X, geometry.GetPosition().Y);
              if (geometry.IsInRange(agtPos) &&
                agt.GetPosition().GetDistance(geoPos) > float.Epsilon) {
              map[agent.Id] = agt;
            }
          }
          return map;
        }
        
        default: return null;
      }
    }

  }
}