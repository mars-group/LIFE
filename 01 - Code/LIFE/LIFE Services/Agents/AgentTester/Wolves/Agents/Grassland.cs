using System;
using System.Collections.Generic;
using System.Linq;
using AgentTester.Wolves.Interactions;
using CommonTypes.DataTypes;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Dummies;
using GenericAgentArchitecture.Interfaces;
using GenericAgentArchitecture.Perception;
using LayerAPI.Interfaces;
using Environment = GenericAgentArchitecture.Dummies.Environment;

namespace AgentTester.Wolves.Agents {
  internal class Grassland : Environment, IGenericDataSource {
      public static readonly Vector2f Boundary = new Vector2f(30, 18);
    private readonly Random _random;
    private int _idCounter;

    public Grassland(bool dbg) : base(new IACLoaderWolves()) {
      _random = new Random();
      _idCounter = 0;
      PrintInformation = dbg;

      // Configure console window for debug information.
      if (PrintInformation) {     
        Console.SetBufferSize(100, 200);
        Console.SetWindowSize(100,  32);
      }
    }


    /// <summary>
    ///   In this simple scenario, there is no need for environmental evolution. 
    ///   Nevertheless, the spawning of some additional grass agents would be nice.
    /// </summary>
    protected override void AdvanceEnvironment() {
      var grassCount = Agents.OfType<Grass>().Count();
      if (_random.Next(40+grassCount) < 20) {
        AddAgent(new Grass(this, "#"+(_idCounter<10? " " : "")+_idCounter));
      }
    }


    /// <summary>
    ///   Add an agent to the execution list. Also set random position.
    /// </summary>
    /// <param name="newAgent">The agent to add.</param>
    public override void AddAgent(Agent newAgent) {
      bool unique;
      do {
        var x = _random.Next((int)Boundary.X);
        var y = _random.Next((int)Boundary.Y);
        newAgent.Position = new Vector2f(x, y);
        unique = true;
        foreach (var agent in Agents) {
          if (agent.Position.X == newAgent.Position.X &&
              agent.Position.Y == newAgent.Position.Y) {
            unique = false;
            break;
          }
        }
      } while (!unique);
      base.AddAgent(newAgent);
      _idCounter++;
    }


    /// <summary>
    ///   Calculate the distance between two agents.
    /// </summary>
    /// <param name="x">The first agent.</param>
    /// <param name="y">The second agent.</param>
    /// <returns>A value describing the distance between these agents.</returns>
    public override double GetDistance(Agent x, Agent y) {
      return x.Position.GetDistance(y.Position);
    }


    /// <summary>
    ///   Check, if a position can be acquired.
    /// </summary>
    /// <param name="position">The intended position</param>
    /// <returns>True, if accessible, false, when not.</returns>
    public bool CheckPosition(Vector2f position)
    {
      if (position.X < 0 || position.X >= Boundary.X ||
          position.Y < 0 || position.Y >= Boundary.Y) return false;
      foreach (var agent in Agents) {
        if (agent.Position.X == position.X &&
            agent.Position.Y == position.Y) return false;
      }
      return true;
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
      for (var i = 0; i < Boundary.X; i ++) str += "─";
      str += "┐\n";
      for (var y = 0; y < Boundary.Y; y ++) {
        str += "│";
        for (var x = 0; x < Boundary.X; x ++) str += " ";
        str += "│\n";
      }
      str += "└";
      for (var i = 0; i < Boundary.X; i ++) str += "─";
      str += "┘";     
      Console.SetCursorPosition(0, 2);
      Console.WriteLine(str);

      // Print the agents.
      foreach (var agent in Agents) {
        Console.SetCursorPosition((int)agent.Position.X+1, (int)agent.Position.Y+3);
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
          if      (((double)g.Foodvalue / Grass.FoodvalueMax) > 0.66) Console.Write("▓");
          else if (((double)g.Foodvalue / Grass.FoodvalueMax) > 0.33) Console.Write("▒");
          else                                                        Console.Write("░");      
        }
      }
      Console.ForegroundColor = ConsoleColor.Gray;

      // Print the individual agent debug messages.
      var xo = (int) Boundary.X + 4;
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
      Console.SetCursorPosition(0, (int)Boundary.Y+4);
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
          var map = new Dictionary<string, Agent>();
          foreach (var agent in GetAllAgents()) {
              if (geometry.IsInRange(agent.Position) &&
                agent.Position.GetDistance(geometry.GetPosition()) > float.Epsilon) {
              map[agent.Id] = agent;
            }
          }
          return map;
        }
        
        default: return null;
      }
    }

  }
}