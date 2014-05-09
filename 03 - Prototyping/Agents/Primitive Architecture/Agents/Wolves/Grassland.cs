using System;
using System.Linq;
using Primitive_Architecture.Dummies;
using Primitive_Architecture.Interactions.Wolves;
using Environment = Primitive_Architecture.Dummies.Environment;

namespace Primitive_Architecture.Agents.Wolves {
  internal class Grassland : Environment {
    public static readonly Vector Boundary = new Vector(25, 20, 0);
    private readonly Random _random;
    private int _idCounter;

    public Grassland() : base(new IACLoaderWolves()) {
      _random = new Random();
      _idCounter = 8;
    }


    /// <summary>
    ///   In this simple scenario, there is no need for environmental evolution. 
    ///   Nevertheless, the spawning of some additional grass agents would be nice.
    /// </summary>
    protected override void AdvanceEnvironment() {
      var grassCount = Agents.OfType<Grass>().Count();
      if (_random.Next(5 + grassCount) < 3) {
        AddAgent(new Grass("[Grass " +_idCounter+"]"));
        _idCounter++;
      }
    }


    /// <summary>
    ///   Add an agent to the execution list. Also set random position.
    /// </summary>
    /// <param name="newAgent">The agent to add.</param>
    public override void AddAgent(Agent newAgent) {
      bool unique;
      do {
        newAgent.Position.X = _random.Next(Boundary.X);
        newAgent.Position.Y = _random.Next(Boundary.Y);
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
  }
}