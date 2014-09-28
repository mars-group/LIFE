using System.Collections.Generic;
using ESCTestLayer.Interface;
using GenericAgentArchitecture.Agents;
using TVector = CommonTypes.DataTypes.Vector;

namespace GenericAgentArchitecture.Movement {
  
  /// <summary>
  ///   This adapter provides ESC usage via generic IEnvironment interface. 
  /// </summary>
  public class ESCAdapter : IEnvironment {

    private readonly IESC _esc;  // Environment Service Component (ESC) implementation.
    private readonly Dictionary<SpatialAgent, MData> _agents; // All registered agents.


    /// <summary>
    ///   Create a new ESC adapter.
    /// </summary>
    /// <param name="esc">The ESC reference.</param>
    public ESCAdapter(IESC esc) {
      _esc = esc;
      _agents = new Dictionary<SpatialAgent, MData>();
    }


    /// <summary>
    ///   Add a new agent to the environment.
    /// </summary>
    /// <param name="agent">The agent to add.</param>
    /// <param name="data">Container with movement data.</param>
    public void AddAgent(SpatialAgent agent, MData data) {
      var dim = data.Dimension;
      _agents.Add(agent, data);
      _esc.Add((int) agent.Id, 0, true, new TVector(dim.X, dim.Y, dim.Z));
      var position = data.Position;
      var direction = data.Direction;
      //TODO if (position == null) position = _esc.s  random stuff here.
      if (direction == null) direction = new Direction(); 
      ChangePosition(agent, position, direction);
    }


    /// <summary>
    ///   Remove an agent from the environment.
    /// </summary>
    /// <param name="agent">The agent to delete.</param>
    public void RemoveAgent(SpatialAgent agent) {
      _esc.Remove((int) agent.Id);
      _agents.Remove(agent);
    }
    

    /// <summary>
    ///   Update the position and heading of an agent. This function is also
    ///   responsible to set the new values to the agent movement container. 
    /// </summary>
    /// <param name="agent">The agent to move.</param>
    /// <param name="position">New position.</param>
    /// <param name="direction">New heading.</param>
    public void ChangePosition(SpatialAgent agent, Vector position, Direction direction) {
      var pos = new TVector(position.X, position.Y, position.Z);
      var dir = direction.GetDirectionalVector();
      var ret = _esc.SetPosition((int) agent.Id, pos, new TVector(dir.X, dir.Y, dir.Z));
      _agents[agent].Position.X = ret.Position.X;
      _agents[agent].Position.Y = ret.Position.Y;
      _agents[agent].Position.Z = ret.Position.Z;
      //TODO Direction und Wahrnehmungsobjekt übernehmen!!
    }
  }
}
