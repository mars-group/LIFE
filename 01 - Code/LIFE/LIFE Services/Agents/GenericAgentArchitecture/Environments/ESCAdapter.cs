﻿using System.Linq;
using CommonTypes.TransportTypes;
using System.Collections.Generic;
using ESCTestLayer.Interface;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Movement;
using LayerAPI.Interfaces;

namespace GenericAgentArchitecture.Environments {
    
  /// <summary>
  ///   This adapter provides ESC usage via generic IEnvironment interface. 
  /// </summary>
  //TODO [ESC bypass] Drop Env2D, remove :base and all "new"'s and "override". 
  public class ESCAdapter : Environment2D, IEnvironment {

    private readonly IESC _esc;  // Environment Service Component (ESC) implementation.
    private readonly Dictionary<SpatialAgent, MovementData> _agents; // All registered agents.

    /// <summary>
    ///   Create a new ESC adapter.
    /// </summary>
    /// <param name="esc">The ESC reference.</param>
    public ESCAdapter(IESC esc) : base(new Vector(30,20)) {
      _esc = esc;
      _agents = new Dictionary<SpatialAgent, MovementData>();
    }


    /// <summary>
    ///   Add a new agent to the environment.
    /// </summary>
    /// <param name="agent">The agent to add.</param>
    /// <param name="pos">The agent's initial position.</param>
    /// <returns>A movement data container with the initial position set.</returns>
    public new MovementData AddAgent(SpatialAgent agent, Vector pos) {
      var mdata = base.AddAgent(agent, pos); 
      _agents.Add(agent, mdata);
      var dim = mdata.Dimension;
      _esc.Add((int) agent.Id, 0, true, new TVector(dim.X, dim.Y, dim.Z));
      ChangePosition(agent, mdata.Position, mdata.Direction);
      return mdata;
    }


    /// <summary>
    ///   Remove an agent from the environment.
    /// </summary>
    /// <param name="agent">The agent to delete.</param>
    public new void RemoveAgent(SpatialAgent agent) {
      base.RemoveAgent(agent);
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
    public new void ChangePosition(SpatialAgent agent, Vector position, Direction direction) {
      var pos = new TVector(position.X, position.Y, position.Z);
      var dir = direction.GetDirectionalVector();
      var ret = _esc.SetPosition((int) agent.Id, pos, new TVector(dir.X, dir.Y, dir.Z));
      _agents[agent].Position.X = ret.Position.X;
      _agents[agent].Position.Y = ret.Position.Y;
      _agents[agent].Position.Z = ret.Position.Z;
      //TODO Direction und Wahrnehmungsobjekt übernehmen!!
      _agents[agent].Direction.SetPitch(direction.Pitch);
      _agents[agent].Direction.SetYaw(direction.Yaw);
      base.ChangePosition(agent, _agents[agent].Position, _agents[agent].Direction);
    }


    /// <summary>
    ///   Retrieve all agents of this environment.
    /// </summary>
    /// <returns>A list of all spatial agents.</returns>
    public new List<SpatialAgent> GetAllAgents() {
      //TODO This functionality should be implemented by the ESC.
      return _agents.Keys.ToList();
    }


    //TODO [ESC bypass] Throw away later, when Env2D is dropped! 
    protected override void AdvanceEnvironment() {}


    /// <summary>
    ///   This function is used by sensors to gather data from this environment.
    ///   In this case, the adapter redirects to the ESC implementation.
    /// </summary>
    /// <param name="informationType">The type of information to sense.</param>
    /// <param name="geometry">The perception range.</param>
    /// <returns>An object representing the percepted information.</returns>
    public override object GetData(int informationType, IGeometry geometry) {
      return _esc.GetData(informationType, geometry);
    }
  }
}
