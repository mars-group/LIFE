using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DalskiAgent.Agents;
using DalskiAgent.Movement;
using DalskiAgent.Perception;
using LayerAPI.Interfaces;

namespace DalskiAgent.Environments {
 
  /// <summary>
  ///   This environment adds movement support to the generic one and contains SpatialAgents. 
  /// </summary>
  public class Environment2D : Environment, IEnvironment {

    protected readonly Vector Boundaries;    // Env. extent, ranging from (0,0) to this point.
    protected readonly ConcurrentDictionary<SpatialAgent, MovementData> Agents;  // Agent-to-movement data mapping.
    public bool IsGrid { get; private set; }                           // Grid-based or continuous?


    /// <summary>
    ///   Create a new 2D environment.
    /// </summary>
    /// <param name="boundaries">Boundaries for the environment.</param>
    /// <param name="isGrid">Selects, if this environment is grid-based or continuous.</param>
    public Environment2D(Vector boundaries, bool isGrid = true) {
      Boundaries = boundaries;
      Agents = new ConcurrentDictionary<SpatialAgent, MovementData>();
      IsGrid = isGrid;
      if (!isGrid) throw new NotImplementedException();
    }


    /// <summary>
    ///   Add an agent to the environment.
    /// </summary>
    /// <param name="agent">The agent to add.</param>
    /// <param name="pos">The agent's initial position.</param>
    /// <param name="mdata">The movement data container reference.</param>
    public void AddAgent(SpatialAgent agent, Vector pos, out MovementData mdata) {
      if (pos == null) pos = GetRandomPosition();
      mdata = new MovementData(pos);
      Agents[agent] = mdata;
      AddAgent(agent);
    }

/*
- Agenten-Positionsabfrage nur über Umwelt. Keine GETs() mehr?
- Abfragemethode, kann internes mdata nutzen.
- "Bulk"-GetData: Liefert readonly MData mit. 
*/


    /// <summary>
    ///   Remove an agent.
    /// </summary>
    /// <param name="agent">The agent to remove.</param>
    public void RemoveAgent(SpatialAgent agent) {
      MovementData m;
      Agents.TryRemove(agent, out m);
      base.RemoveAgent(agent);
    }


    /// <summary>
    ///   Set a new position and direction of an agent.
    /// </summary>
    /// <param name="agent">The moving agent.</param>
    /// <param name="position">New position to acquire.</param>
    /// <param name="direction">New heading.</param>
    public void ChangePosition(SpatialAgent agent, Vector position, Direction direction) {
      
      // Return, if position is already blocked.
      if (! CheckPosition(position) || !Agents.ContainsKey(agent)) return;
      // Set the values.
      Agents[agent].Position.X = position.X;
      Agents[agent].Position.Y = position.Y;
      Agents[agent].Position.Z = position.Z;
      Agents[agent].Direction.SetPitch(direction.Pitch);
      Agents[agent].Direction.SetYaw(direction.Yaw);
    }


    /// <summary>
    ///   Retrieve all agents of this environment.
    /// </summary>
    /// <returns>A list of all spatial agents.</returns>
    public new List<SpatialAgent> GetAllAgents() {
      return new List<SpatialAgent>(Agents.Keys);
    }


    /// <summary>
    ///   This function allows execution of environment-specific code.
    ///   The generic 2D environment does not use it. Later override possible.
    /// </summary>
    protected override void AdvanceEnvironment() {}


    /// <summary>
    ///   Returns a random position.
    /// </summary>
    /// <returns>A free position.</returns>
    public Vector GetRandomPosition() {
      if (IsGrid) {
        bool unique;
        Vector position;
        do {
          var x = Random.Next((int)Boundaries.X);
          var y = Random.Next((int)Boundaries.Y);
          position = new Vector(x, y);
          unique = true;
          foreach (var md in Agents.Values) {
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
      foreach (var md in Agents.Values) {
        if (md.Position.Equals(position)) return false;
      }
      return true;
    }


    /// <summary>
    ///   This function is used by sensors to gather data from this environment.
    ///   It contains a function for "0: Get all perceptible agents". Further refinement 
    ///   can be made by specific environments overriding this function. 
    /// </summary>
    /// <param name="informationType">The type of information to sense.</param>
    /// <param name="geometry">The perception range.</param>
    /// <returns>An object representing the percepted information.</returns>
    public virtual object GetData(int informationType, IGeometry geometry) {
      switch (informationType) {
        case 0: { // Zero stands here for "all agents". Enum avoided, check it elsewhere!
          var halo = (Halo) geometry;
          return GetAllAgents().Where(agent => 
            halo.IsInRange(agent.GetPosition().GetTVector()) && 
            halo.Position.GetDistance(agent.GetPosition()) > float.Epsilon).ToList();
        }

        // Throw exception, if wrong argument was supplied.
        default: throw new Exception(
          "[Environment2D] Error on GetData call. Queried '"+informationType+"', though "+
          "only '0' is valid. Please make sure to override function in specific environment!");
      }
    }
  }
}
