using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DalskiAgent.Agents;
using DalskiAgent.Movement;
using DalskiAgent.Perception;
using GenericAgentArchitectureCommon.Interfaces;

namespace DalskiAgent.Environments {
 
  /// <summary>
  ///   This environment adds movement support to the generic one and contains SpatialAgents. 
  /// </summary>
  public class Environment2D : IEnvironment {
    
    private readonly Vector _boundaries;    // Env. extent, ranging from (0,0) to this point.
    private readonly bool _isGrid;          // Grid-based or continuous environment?    
    private readonly Random _random;        // Number generator for random placement.
    protected readonly ConcurrentDictionary<SpatialAgent, MovementData> Agents;  // Agent-to-movement data mapping.


    /// <summary>
    ///   Create a new 2D environment.
    /// </summary>
    /// <param name="boundaries">Boundaries for the environment.</param>
    /// <param name="isGrid">Selects, if this environment is grid-based or continuous.</param>
    public Environment2D(Vector boundaries, bool isGrid = true) {
      _boundaries = boundaries;
      _random = new Random();
      Agents = new ConcurrentDictionary<SpatialAgent, MovementData>();
      _isGrid = isGrid;
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
    }


    /// <summary>
    ///   Remove an agent.
    /// </summary>
    /// <param name="agent">The agent to remove.</param>
    public void RemoveAgent(SpatialAgent agent) {
      MovementData m;
      Agents.TryRemove(agent, out m);     
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
    ///   This function allows execution of environment-specific code.
    ///   The generic 2D environment does not use it. Later override possible.
    /// </summary>
    public virtual void AdvanceEnvironment() {}


    /// <summary>
    ///   Retrieve all agents of this environment.
    /// </summary>
    /// <returns>A list of all spatial agents.</returns>
    public List<SpatialAgent> GetAllAgents() {
      return new List<SpatialAgent>(Agents.Keys);
    }


    /// <summary>
    ///   Returns a random position.
    /// </summary>
    /// <returns>A free position.</returns>
    public Vector GetRandomPosition() {
      if (_isGrid) {
        bool unique;
        Vector position;
        do {
          var x = _random.Next((int)_boundaries.X);
          var y = _random.Next((int)_boundaries.Y);
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
    ///   Generate a valid initial movement data container for some given criteria. 
    /// </summary>
    /// <param name="dim">Extents of object to place (ranging from (0,0,0) to this point).</param>
    /// <param name="dir">Direction of object. If not set, it's chosen randomly.</param>
    /// <param name="target">A wished starting position. It's tried to find a fitting position as close as possible.</param>
    /// <param name="maxRng">Maximum allowed range to target position. Ignored, if no target specified.</param>
    /// <returns>A movement data object meeting the given requirements. 'Null', if placement not possible!</returns>
    private MovementData PlaceAtRandomFreePosition(Vector dim, Direction dir = null, Vector target = null, float maxRng = 1) {
      throw new NotImplementedException();
    }


/* WARUM DAS SO MIST IST UND WIE ES BESSER WÄRE:
 * 
 * - Env bekommt fertigberechnete Zielposition
 * - muß dann irgendwie gucken, ob's geht
 * - was wenn nicht? Wie weit kommt der Agent dann? Bewegungsgerade bilden und dann schneiden?
 * 
 * Besser: 
 
 */


    /// <summary>
    ///   Check, if a position can be acquired.
    /// </summary>
    /// <param name="position">The intended position</param>
    /// <returns>True, if accessible, false, when not.</returns>
    public bool CheckPosition(Vector position) {
      if (position.X < 0 || position.X >= _boundaries.X ||
          position.Y < 0 || position.Y >= _boundaries.Y) return false;
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
    /// <param name="deprecatedGeometry">The perception range.</param>
    /// <returns>An object representing the percepted information.</returns>
    public virtual object GetData(int informationType, IDeprecatedGeometry deprecatedGeometry) {
      switch (informationType) {
        case 0: { // Zero stands here for "all agents". Enum avoided, check it elsewhere!
          var halo = (Halo) deprecatedGeometry;
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
