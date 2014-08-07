using System.Collections.Generic;
using LayerAPI.Interfaces;
namespace AgentTester.RandomMove_ESC {

  internal class WalkerLayer : ISteppedLayer, ITickClient {
    
    private readonly List<ITickClient> _agents;   // List of all agents.
    private long _tick;                           // Current tick count.


    /// <summary>
    /// Create a new layer for random agents.
    /// </summary>
    /// <param name="agents">List with all agents to execute.</param>
    public WalkerLayer(List<ITickClient> agents) {
      _tick = 0;
      _agents = agents;
    }



    /// <summary>
    ///   Initialize the layer.
    /// </summary>
    public bool InitLayer<T>(T layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
      foreach (var agent in _agents) {
        registerAgentHandle.Invoke (this, agent);
      }    
      return true;
    }



    /// <summary>
    ///   Advances layer and all agents on it.
    /// </summary>
    public void Tick() {
      foreach (var agent in _agents) agent.Tick();
      _tick ++;
    }



    /// <summary>
    ///   Return the current tick count. Needed by layer interface (for what ever...).
    /// </summary>
    /// <returns>The current tick.</returns>
    public long GetCurrentTick() {
      return _tick;
    }
  }
}