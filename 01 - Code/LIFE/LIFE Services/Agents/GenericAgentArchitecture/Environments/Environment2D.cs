using System;
using System.Collections.Generic;
using System.Linq;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Movement;
using LayerAPI.Interfaces;

namespace GenericAgentArchitecture.Environments {
    using CommonTypes.DataTypes;
    using CommonTypes.TransportTypes;

    /// <summary>
  ///   This environment adds movement support to the generic one and contains SpatialAgents. 
  /// </summary>
  public abstract class Environment2D : Environment, IEnvironment {

    protected readonly Vector Boundaries;    // Env. extent, ranging from (0,0) to this point.
    protected readonly Dictionary<SpatialAgent, MData> Agents;  // Agent-to-movement data mapping.
    public bool IsGrid { get; private set; }                    // Grid-based or continuous?


    /// <summary>
    ///   Create a new 2D environment.
    /// </summary>
    /// <param name="boundaries">Boundaries for the environment.</param>
    /// <param name="isGrid">Selects, if this environment is grid-based or continuous.</param>
    public Environment2D(Vector boundaries, bool isGrid = true) {
      Boundaries = boundaries;
      Agents = new Dictionary<SpatialAgent, MData>();
      IsGrid = isGrid;
      if (!isGrid) throw new NotImplementedException();
    }


    /// <summary>
    ///   Add an agent to the environment.
    /// </summary>
    /// <param name="agent">The agent to add.</param>
    /// <param name="data">It's movement data.</param>
    public void AddAgent(SpatialAgent agent, MData data) {
      Agents.Add(agent, data);
      AddAgent(agent);
    }


    /// <summary>
    ///   Remove an agent.
    /// </summary>
    /// <param name="agent">The agent to remove.</param>
    public void RemoveAgent(SpatialAgent agent) {
      Agents.Remove(agent);
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
      if (! CheckPosition(position)) return;
      
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
      return base.GetAllAgents().Cast<SpatialAgent>().ToList();
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
    /// </summary>
    /// <param name="informationType">The type of information to sense.</param>
    /// <param name="geometry">The perception range.</param>
    /// <returns>An object representing the percepted information.</returns>
    public abstract object GetData(int informationType, IGeometry geometry);
  }
}
