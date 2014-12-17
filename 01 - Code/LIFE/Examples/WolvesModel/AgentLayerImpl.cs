using DalskiAgent.Execution;
using LifeAPI.Spatial;
using LifeAPI.Layer;
using Mono.Addins;
using WolvesModel.Agents;
using WolvesModel.Environment;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]


namespace WolvesModel {

  /// <summary>
  ///   Data query information types.
  /// </summary>
  public enum InformationTypes { AllAgents, Grass, Sheeps, Wolves }
    

  /// <summary>
  ///   This layer implementation contains a predator-prey simulation (wolves vs. sheeps vs. grass).
  ///   It uses the Generic Agent Architecture and serves as an example for other agent models.
  /// </summary>
  [Extension(typeof (ISteppedLayer))]
  public class AgentLayerImpl : ISteppedLayer {

    private Grassland _env;    // Grassland (environment) object for spatial agents. 
    private IExecution _exec;  // Agent execution container reference.
    private long _tick;        // Current tick.


    /// <summary>
    ///   Initializes this layer.
    /// </summary>
    /// <typeparam name="T">Object type of layer init data object.</typeparam>
    /// <param name="layerInitData">Generic layer init data object. Not used here!</param>
    /// <param name="registerAgentHandle">Delegate for agent registration function.</param>
    /// <param name="unregisterAgentHandle">Delegate for agent unregistration function.</param>
    /// <returns></returns>
    public bool InitLayer<T>(T layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {  

      // Create the environment, the execution container and an agent spawner.
      _env = new Grassland(new Vector(100, 100), true);
      _exec = new LayerExec(registerAgentHandle, unregisterAgentHandle, this);      
      new AgentSpawner(_exec, _env);

      const int j = 2;

      // Create some initial agents.
      for (var i = 0; i < 5*j; i ++) new Grass(_exec, _env);
      for (var i = 0; i < 3*j; i ++) new Sheep(_exec, _env);
      for (var i = 0; i < 2*j; i ++) new Wolf (_exec, _env);     
      return true;
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
