using AgentTester.Wolves.Agents;
using AgentTester.Wolves.Environment;
using DalskiAgent.Execution;
using GenericAgentArchitectureCommon.Datatypes;
using LayerAPI.Interfaces;
using Mono.Addins;
using SpatialCommon.Datatypes;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]


namespace AgentTester {

  /// <summary>
  ///   Data query information types.
  /// </summary>
  public enum InformationTypes { AllAgents, Grass, Sheeps, Wolves }
}


namespace AgentTester.Wolves {       

  /// <summary>
  ///   This layer implementation contains a predator-prey simulation (wolves vs. sheeps vs. grass).
  ///   It uses the Generic Agent Architecture and serves as an example for other agent models.
  /// </summary>
  [Extension(typeof (ISteppedLayer))]
  public class AgentLayerImpl : ISteppedLayer {

    private Grassland _env;          // Grassland (environment) object for spatial agents. 
    private IExecution _exec;        // Agent execution container reference.
    private AgentSpawner _spawner;   // Agent spawner object.


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
      _env = new Grassland(true, new Vector(100, 100), true);
      _exec = new LayerExec(registerAgentHandle, unregisterAgentHandle, this);      
      _spawner = new AgentSpawner(_exec, _env);

      int j = 1;

      // Create some initial agents.
      for (var i = 0; i < 50*j; i ++) new Grass(_exec, _env);
      for (var i = 0; i < 30*j; i ++) new Sheep(_exec, _env);
      for (var i = 0; i < 20*j; i ++) new Wolf (_exec, _env);     
      return true;
    }


    /// <summary>
    ///   Returns the current tick.
    /// </summary>
    /// <returns>Current tick value.</returns>
    public long GetCurrentTick() {
      return _spawner.TickCnt;
    }

    public void SetCurrentTick(long currentTick) {
      throw new System.NotImplementedException();
    }
  }
}
