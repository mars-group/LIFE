using DalskiAgent.Agents;
using LifeAPI.Layer;

namespace DalskiAgent.Execution {
  
  /// <summary>
  ///   An execution implementation that uses the MARS layer and delegates.
  /// </summary>
  public class LayerExec : IExecution {

    private readonly RegisterAgent _regFkt;     // Agent registration function pointer.
    private readonly UnregisterAgent _unregFkt; // Delegate for unregistration function.
    private readonly ILayer _layerImpl;         // Layer reference needed for delegate calls.    
    private static long _idCounter;             // Counter for agent ID distribution.
    private static readonly object Lock = new object();  // ID access synchronization flag.

    /// <summary>
    ///   Create a new execution class for use with MARS layers.
    /// </summary>
    /// <param name="regFkt">Delegate for agent registration function.</param>
    /// <param name="unregFkt">Delegate for agent unregistration function.</param>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    public LayerExec(RegisterAgent regFkt, UnregisterAgent unregFkt, ILayer layer) {
      _regFkt = regFkt;
      _unregFkt = unregFkt;
      _layerImpl = layer;
    }
    

    /// <summary>
    ///   Adds an agent to the layer by registering for execution. 
    /// </summary>
    /// <param name="agent">The agent to add.</param>
    public void AddAgent(Agent agent) {
      _regFkt(_layerImpl, agent);
    }


    /// <summary>
    ///   Removes an agent from the execution list.
    /// </summary>
    /// <param name="agent">The agent to remove.</param>
    public void RemoveAgent(Agent agent) {
      _unregFkt(_layerImpl, agent);
    }


    /// <summary>
    ///   Return an ID for a new agent.
    /// </summary>
    /// <returns>A unique identifier.</returns>
    public long GetNewID() {
      lock (Lock) {
        return _idCounter++;
      }
    }


    /// <summary>
    ///   Return the current simulation tick.
    /// </summary>
    /// <returns>Current tick counter value.</returns>
    public long GetCurrentTick() {
      return ((ISteppedLayer) _layerImpl).GetCurrentTick();
    }
  }
}
