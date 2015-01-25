using System;
using System.Collections.Generic;
using EnvironmentServiceComponent.Implementation;
using LCConnector.TransportTypes;
using LifeAPI.Environment;
using LifeAPI.Spatial;
using LifeAPI.Layer;
using Mono.Addins;
using WolvesModel.Agents;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace WolvesModel {    

  /// <summary>
  ///   This layer implementation contains a predator-prey simulation (wolves vs. sheeps vs. grass).
  ///   It uses the Generic Agent Architecture and serves as an example for other agent models.
  /// </summary>
  [Extension(typeof (ISteppedLayer))]
  public class AgentLayerImpl : ISteppedLayer {

    private IEnvironment _env;  // Grassland (environment) object for spatial agents. 
    private long _tick;         // Current tick.


    /// <summary>
    ///   Initializes this layer.
    /// </summary>
    /// <param name="layerInitData">Generic layer init data object. Not used here!</param>
    /// <param name="registerAgentHandle">Delegate for agent registration function.</param>
    /// <param name="unregisterAgentHandle">Delegate for agent unregistration function.</param>
    /// <returns></returns>
    public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {  

      // Create the environment, the execution container and an agent spawner.
      _env = new RectESC {IsGrid = true, MaxDimension = new TVector(30, 20)};

      // ReSharper disable ObjectCreationAsStatement
      new AgentSpawner(this, registerAgentHandle, unregisterAgentHandle, _env);
      const int j = 2;

      // Create some initial agents.
      for (var i = 0; i < 5*j; i ++) new Grass(this, registerAgentHandle, unregisterAgentHandle, _env);
      for (var i = 0; i < 3*j; i ++) new Sheep(this, registerAgentHandle, unregisterAgentHandle, _env);
      for (var i = 0; i < 2*j; i ++) new Wolf (this, registerAgentHandle, unregisterAgentHandle, _env);     
      
      // ReSharper restore ObjectCreationAsStatement
      return true;
    }


    /// <summary>
    ///   Updates all shadow agents.
    /// </summary>
    /// <param name="agentsToAdd">Agents to add.</param>
    /// <param name="agentsToRemove">Agent to remove.</param>
    public void UpdateShadowAgents(IDictionary<Type, List<Guid>> agentsToAdd, IDictionary<Type, List<Guid>> agentsToRemove) {
      throw new NotImplementedException();
    }


    /// <summary>
    ///   Returns the current tick.
    /// </summary>
    /// <returns>Current tick value.</returns>
    public long GetCurrentTick() {
      return _tick;
    }


    /// <summary>
    ///   Sets the current tick. This function is called by the RTE manager in each tick.
    /// </summary>
    /// <param name="currentTick">current tick value.</param>
    public void SetCurrentTick(long currentTick) {
      _tick = currentTick;
    }
  }
}
