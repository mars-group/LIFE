using System.Collections.Generic;
using DalskiAgent.Agents;
using DalskiAgent.Movement;
using ESCTestLayer.Interface;
using LayerAPI.Interfaces;

namespace DalskiAgent.Environments {
  
  /// <summary>
  ///   An environment implementation for the spatial agents to use.
  ///   It wraps the add/remove delegate calls and the ESC functions. 
  /// </summary>
  public class LayerEnvironment : IEnvironment {

    private readonly IEnvironment _env;         // ESC adapter (or own environment).
    private readonly RegisterAgent _regFkt;     // Agent registration function pointer.
    private readonly UnregisterAgent _unregFkt; // Delegate for unregistration function.
    private readonly ILayer _layerImpl;         // Layer reference needed for delegate calls.


    /// <summary>
    ///   Create a new environment class for use with MARS layers.
    /// </summary>
    /// <param name="esc">The ESC for collision detection. May be set to null for own environment.</param>
    /// <param name="regFkt">Delegate for agent registration function.</param>
    /// <param name="unregFkt">Delegate for agent unregistration function.</param>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    public LayerEnvironment(IDeprecatedESC esc, RegisterAgent regFkt, UnregisterAgent unregFkt, ILayer layer) {
      if (esc == null) _env = new Environment2D(new Vector(30, 20));
      else _env = new ESCAdapter(esc);
      _regFkt = regFkt;
      _unregFkt = unregFkt;
      _layerImpl = layer;
    }


    /// <summary>
    ///   Adds an agent to the layer. It is registered for execution and added to the ESC. 
    /// </summary>
    /// <param name="agent">The agent to add.</param>
    /// <param name="pos">The agent's initial position.</param>
    /// <param name="mdata">The movement data container reference.</param>
    public void AddAgent(SpatialAgent agent, Vector pos, out MovementData mdata) {
      _env.AddAgent(agent, pos, out mdata);
      _regFkt(_layerImpl, agent);
    }


    /// <summary>
    ///   Removes an agent from the execution list and the environment.
    /// </summary>
    /// <param name="agent">The agent to remove.</param>
    public void RemoveAgent(SpatialAgent agent) {
      _unregFkt(_layerImpl, agent);
      _env.RemoveAgent(agent);
    }


    /// <summary>
    ///   Update an agent's movement data. This function delegates to the ESC adapter.
    /// </summary>
    /// <param name="agent">The agent to move.</param>
    /// <param name="position">New position.</param>
    /// <param name="direction">New heading.</param>
    public void ChangePosition(SpatialAgent agent, Vector position, Direction direction) {
      _env.ChangePosition(agent, position, direction);
    }


    /// <summary>
    ///   This function is used by sensors to gather data from this environment.
    ///   In this case, the adapter redirects to the ESC implementation.
    /// </summary>
    /// <param name="informationType">The type of information to sense.</param>
    /// <param name="geometry">The perception range.</param>
    /// <returns>An object representing the percepted information.</returns>
    public object GetData(int informationType, IGeometry geometry) {
      return _env.GetData(informationType, geometry);
    }


    /// <summary>
    ///   Retrieve all agents of this environment.
    /// </summary>
    /// <returns>A list of all spatial agents.</returns>
    public List<SpatialAgent> GetAllAgents() {
      return _env.GetAllAgents();
    }
  }
}
