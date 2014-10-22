using System;
using System.Collections.Generic;
using System.Linq;
using AgentTester.Wolves.Agents;
using CommonTypes.TransportTypes;
using ESCTestLayer.Implementation;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Environments;
using GenericAgentArchitecture.Movement;
using LayerAPI.Interfaces;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace AgentTester.Wolves {

  /// <summary>
  ///   Enumeration of information types that can be queried.
  /// </summary>
  enum InformationTypes { GetAllAgents, Grass, Sheeps, Wolves }


  /// <summary>
  ///   This layer implementation contains a predator-prey simulation (wolves vs. sheeps vs. grass).
  ///   It uses the Generic Agent Architecture and serves as an example for other agent models.
  /// </summary>
  public class AgentLayerImpl : ISteppedLayer, IEnvironment, ITickClient {

    private long _tick;                     // Counter of current tick.
    private List<SpatialAgent> _agents;     // List of contained agents.
    private Random _random;                 // Random number generator.
    private ESCAdapter _esc;                // Environment Service Component (ESC) adapter.
    private RegisterAgent _regFunction;     // Agent registration function (delegate).
    private UnregisterAgent _unregFunction; // Unregistration function.
    private long _idCounter;                // Agent ID counter. Auto-incremented on each Add() call.


    /// <summary>
    ///   Initializes this layer.
    /// </summary>
    /// <typeparam name="T">Object type of layer init data object.</typeparam>
    /// <param name="layerInitData">Generic layer init data object. Not used here!</param>
    /// <param name="registerAgentHandle">Delegate for agent registration function.</param>
    /// <param name="unregisterAgentHandle"></param>
    /// <returns></returns>
    public bool InitLayer<T>(T layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
      _random = new Random();
      _agents = new List<SpatialAgent>();
      _regFunction = registerAgentHandle;
      _unregFunction = unregisterAgentHandle;
      _esc = new ESCAdapter(new ESC());
      _idCounter = 0;
      _tick = 0;

      // Create some initial agents.
      //TODO    → → →   ESC come on! I need this function! Now!!   ← ← ←
      for (var i = 0; i < 12; i ++) new Grass(_idCounter, this, new TVector(0,i,0));
//      for (var i = 0; i <  6; i ++) new Sheep(_idCounter, this, GetRandomPosition());
//      for (var i = 0; i <  2; i ++) new Wolf (_idCounter, this, GetRandomPosition());
     
      // Register the layer itself for execution. The agents are registered by themselves (look comment below).
      registerAgentHandle.Invoke(this, this);
      return true;
    }


    /// <summary>
    ///   This layer is also tickable to execute some functions.
    ///   It increases the tick counter and spawns some additional agents.
    /// </summary>
    public void Tick() {
      var grassCount = _agents.OfType<Grass>().Count();
      var create = _random.Next(40 + grassCount) < 20;
//      if (create) new Grass(_idCounter, this, GetRandomPosition());
      _tick ++;
    }


    /// <summary>
    ///   Returns data from the environment.
    /// </summary>
    /// <param name="informationType">The information to query.</param>
    /// <param name="geometry">The perception geometry. In this case, an agent halo.</param>
    /// <returns>A list of agents that are in perception range.</returns>
    public object GetData(int informationType, IGeometry geometry) {
      return _esc.GetData(informationType, geometry);
      /*
      switch ((InformationTypes) informationType) {      
        case InformationTypes.GetAllAgents: {
          var map = new Dictionary<long, SpatialAgent>();
          var halo = (Halo) geometry;
          foreach (var agent in _agents) {
            if (halo.IsInRange(agent.GetPosition().GetTVector()) &&
                halo.Position.GetDistance(agent.GetPosition()) > float.Epsilon) {
              map[agent.Id] = agent;
            }
          }
          return map;
        }       
        default: return null;
      }
      */
    }


    /// <summary>
    ///   Returns the current tick.
    /// </summary>
    /// <returns>Current tick value.</returns>
    public long GetCurrentTick() {
      return _tick;
    }


    /* NOTE: The following functions may cause some confusion. For clarification:
     * The generic agent receives an environmental reference and takes care of registration and 
     * unregistration. In this setup, this layer is both agent creator and environment, and also 
     * contains an ESC adapter to provide collision detection. 
     * So the base agent automatically calls this add/remove functions and they in turn redirect
     * the calls to the ESC and the execution environment.
     */


    /// <summary>
    ///   This function is called *** by the agent *** on construction.
    ///   It adds the agent to the list and registers it at the ESC and the execution service. 
    /// </summary>
    /// <param name="agent">The agent to add.</param>
    /// <param name="data">agent movement data.</param>
    public void AddAgent(SpatialAgent agent, MovementData data) {  
      _esc.AddAgent(agent, data);
      _agents.Add(agent);
      _regFunction.Invoke(this, agent);
      _idCounter ++;
    }


    /// <summary>
    ///   Remove an agent from the environment.
    /// </summary>
    /// <param name="agent">The agent to delete.</param>
    public void RemoveAgent(SpatialAgent agent) {
      _esc.RemoveAgent(agent);
      _agents.Remove(agent);
      _unregFunction.Invoke(this, agent);
    }


    /// <summary>
    ///   Update the position and heading of an agent. It is a redirection to the ESC.
    /// </summary>
    /// <param name="agent">The agent to move.</param>
    /// <param name="position">New position.</param>
    /// <param name="direction">New heading.</param>
    public void ChangePosition(SpatialAgent agent, Vector position, Direction direction) {
      _esc.ChangePosition(agent, position, direction);
    }


    /// <summary>
    ///   Retrieve all agents of this environment.
    /// </summary>
    /// <returns>A list of all spatial agents.</returns>
    public List<SpatialAgent> GetAllAgents() {
      return new List<SpatialAgent>(_agents);
    }
  }
}
