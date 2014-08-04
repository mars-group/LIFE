using System;
using System.Collections.Generic;
using AgentTester.Wolves.Agents;
using AgentTester.Wolves.Interactions;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Dummies;

namespace AgentTester.Wolves.Reasoning {

  /// <summary>
  ///   Static class containing common methods used in the reasoning unit. 
  /// </summary>
  internal static class CommonRCF {

    private const int MoveRng = 6;                        // Interval for random number. 
    private static readonly Random Random = new Random(); // Random number generator for movement.


    /// <summary>
    ///   Perform the random movement of an agent in a 2D grid environment.
    /// </summary>
    /// <param name="env">The environment (needed to check if new position is valid).</param>
    /// <param name="agent">The agent that shall be moved.</param>
    /// <returns>A movement interaction (or null, if agent stays in place).</returns>
    public static MoveInteraction GetRandomMoveInteraction(Grassland env, Agent agent) {     
      var curPos = agent.Position;
      Vector newPos;
      do {
        var rnd = Random.Next(MoveRng);
        switch (rnd) {
          case 0: newPos = new Vector(curPos.X, curPos.Y-1); break;
          case 1: newPos = new Vector(curPos.X, curPos.Y+1); break;
          case 2: newPos = new Vector(curPos.X-1, curPos.Y); break;
          case 3: newPos = new Vector(curPos.X+1, curPos.Y); break;
          default: return null;
        }
      } while (!env.CheckPosition(newPos));
      return new MoveInteraction(agent, newPos);
    }


    /// <summary>
    ///   Try to move towards a target.
    /// </summary>
    /// <param name="env">The environment (needed to check if new position is valid).</param>
    /// <param name="agent">Self reference to the calling agent.</param>
    /// <param name="targetPos">The position to go to.</param>
    /// <returns>A movement interaction (or null, if agent stays in place).</returns>
    public static MoveInteraction MoveTowardsPosition(Grassland env, Agent agent, Vector targetPos) {
      Vector newPos;
      var curPos = agent.Position;
      var candidates = new List<Vector>();
      newPos = new Vector(curPos.X, curPos.Y-1); if (env.CheckPosition(newPos)) candidates.Add(newPos);
      newPos = new Vector(curPos.X, curPos.Y+1); if (env.CheckPosition(newPos)) candidates.Add(newPos);
      newPos = new Vector(curPos.X-1, curPos.Y); if (env.CheckPosition(newPos)) candidates.Add(newPos);
      newPos = new Vector(curPos.X+1, curPos.Y); if (env.CheckPosition(newPos)) candidates.Add(newPos);
      if (candidates.Count == 0 || curPos.GetDistance(targetPos) <= 1) return null;

      var bestPos = candidates[0];
      var shortestDistance = bestPos.GetDistance(targetPos);
      for (var i = 1; i < candidates.Count; i++) {
        if (candidates[i].GetDistance(targetPos) < shortestDistance) {
          bestPos = candidates[i];
          shortestDistance = candidates[i].GetDistance(targetPos);
        }
      }
      return new MoveInteraction(agent, bestPos);
    }


    /// <summary>
    ///   Get the nearest agent out of an agent list.
    /// </summary>
    /// <typeparam name="T">The specified agent type used in the list.</typeparam>
    /// <param name="agents">The list itself.</param>
    /// <param name="pos">The base position.</param>
    /// <returns>Reference of the nearest agent - or null, if list is empty.</returns>
    public static T GetNearestAgent<T>(List<T> agents, Vector pos) where T : Agent {
      if (agents.Count == 0) return null;
      Agent nearestAgent = agents[0];
      var minDistance = pos.GetDistance(nearestAgent.Position);
      for (var i = 1; i < agents.Count; i++) {
        if (pos.GetDistance(agents[i].Position) < minDistance) {
          nearestAgent = agents[i];
          minDistance = pos.GetDistance(nearestAgent.Position);
        }
      }
      return (T) nearestAgent;
    }

  }
}
