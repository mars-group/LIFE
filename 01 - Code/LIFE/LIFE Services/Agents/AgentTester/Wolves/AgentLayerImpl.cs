using System;
using System.Linq;
using AgentTester.Wolves.Agents;
using ESCTestLayer.Implementation;
using GenericAgentArchitecture.Environments;
using LayerAPI.Interfaces;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace AgentTester.Wolves {

  /// <summary>
  ///   Data query information types.
  /// </summary>
  public enum InformationTypes { AllAgents, Grass, Sheeps, Wolves }     


  /// <summary>
  ///   This layer implementation contains a predator-prey simulation (wolves vs. sheeps vs. grass).
  ///   It uses the Generic Agent Architecture and serves as an example for other agent models.
  /// </summary>
  public class AgentLayerImpl : ISteppedLayer, ITickClient {

    private long _tick;            // Counter of current tick.    
    private Random _random;        // Random number generator.
    private long _idCounter;       // Agent ID counter. Auto-incremented on each Add() call.    
    private LayerEnvironment _env; // Environment object for spatial agents. 


    /// <summary>
    ///   Initializes this layer.
    /// </summary>
    /// <typeparam name="T">Object type of layer init data object.</typeparam>
    /// <param name="layerInitData">Generic layer init data object. Not used here!</param>
    /// <param name="registerAgentHandle">Delegate for agent registration function.</param>
    /// <param name="unregisterAgentHandle">Delegate for agent unregistration function.</param>
    /// <returns></returns>
    public bool InitLayer<T>(T layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {  
      _tick = 0;
      _idCounter = 0;
      _random = new Random();
      _env = new LayerEnvironment(null, registerAgentHandle, unregisterAgentHandle, this);

      // Create some initial agents.
      for (var i = 0; i < 12; i ++) new Grass(_idCounter++, _env);
      for (var i = 0; i <  6; i ++) new Sheep(_idCounter++, _env);
      for (var i = 0; i <  2; i ++) new Wolf (_idCounter++, _env);

      // Register the layer itself for execution. The agents are registered by themselves.
      registerAgentHandle.Invoke(this, this);
      return true;
    }


    /// <summary>
    ///   This layer is also tickable to execute some functions.
    ///   It increases the tick counter and spawns some additional agents.
    /// </summary>
    public void Tick() {
      var grassCount = _env.GetAllAgents().OfType<Grass>().Count();
      var create = _random.Next(40 + grassCount) < 20;
      if (create) new Grass(_idCounter++, _env);   
      _tick ++;
      //TODO Output function needed!
    }


    /// <summary>
    ///   Returns the current tick.
    /// </summary>
    /// <returns>Current tick value.</returns>
    public long GetCurrentTick() {
      return _tick;
    }
  }
}
