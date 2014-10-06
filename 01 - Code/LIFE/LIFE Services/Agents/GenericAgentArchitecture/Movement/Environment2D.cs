using System;
using System.Collections.Generic;
using GenericAgentArchitecture.Agents;
using TVector = CommonTypes.DataTypes.Vector;

namespace GenericAgentArchitecture.Movement {
  
  public abstract class Environment2D : Environment, IEnvironment {

    protected readonly Vector Boundaries;    // Env. extent, ranging from (0,0) to this point.
    private readonly Dictionary<SpatialAgent, MData> _agents;  // Agent-to-movement data mapping.
    protected readonly Random Random;                          // Random number generator.
    public bool IsGrid { get; private set; }                   // Grid-based or continuous?


    /// <summary>
    ///   Create a new 2D environment.
    /// </summary>
    /// <param name="boundaries">Boundaries for the environment.</param>
    /// <param name="isGrid">Selects, if this environment is grid-based or continuous.</param>
    public Environment2D(Vector boundaries, bool isGrid = true) {
      Boundaries = boundaries;
      Random = new Random();
      _agents = new Dictionary<SpatialAgent, MData>();
      IsGrid = isGrid;
      if (!isGrid) throw new NotImplementedException();
    }


    /// <summary>
    ///   Add an agent to the environment.
    /// </summary>
    /// <param name="agent">The agent to add.</param>
    /// <param name="data">It's movement data.</param>
    public void AddAgent(SpatialAgent agent, MData data) {
      _agents.Add(agent, data);
      AddAgent(agent);
    }


    /// <summary>
    ///   Remove an agent.
    /// </summary>
    /// <param name="agent">The agent to remove.</param>
    public void RemoveAgent(SpatialAgent agent) {
      _agents.Remove(agent);
      base.RemoveAgent(agent);
    }


    /// <summary>
    ///   Set a new position and direction of an agent.
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="position"></param>
    /// <param name="direction"></param>
    public void ChangePosition(SpatialAgent agent, Vector position, Direction direction) {
      //TODO Fehlt halt noch! Kollisionsvermeidung (für Wölfe benötigt).
      throw new NotImplementedException();
    }


    /// <summary>
    ///   Returns a random position.
    /// </summary>
    /// <returns>A free position.</returns>
    public TVector GetRandomPosition() {
      if (IsGrid) {
        bool unique;
        TVector position;
        do {
          var x = Random.Next((int)Boundaries.X);
          var y = Random.Next((int)Boundaries.Y);
          position = new TVector(x, y);
          unique = true;
          foreach (var md in _agents.Values) {
            if (md.Position.Equals(position)) {
              unique = false;
              break;              
            }
          }
        } while (!unique);
        return position;
      }

      //TODO 
      throw new NotImplementedException();
    }


    /// <summary>
    ///   Check, if a position can be acquired.
    /// </summary>
    /// <param name="position">The intended position</param>
    /// <returns>True, if accessible, false, when not.</returns>
    public bool CheckPosition(Vector position) {
      if (position.X < 0 || position.X >= Boundaries.X ||
          position.Y < 0 || position.Y >= Boundaries.Y) return false;
      //TODO Dimensional checks needed!
      foreach (var md in _agents.Values) {
        if (md.Position.Equals(position)) return false;
      }
      return true;
    }
  }
}
