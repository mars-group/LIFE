using LIFE.API.GridCommon;
using LIFE.API.Layer;
using LIFE.API.Results;
using LIFE.Components.Agents.DalskiAgent.Environment;
using LIFE.Components.Agents.DalskiAgent.Movement;
using LIFE.Components.Environments.GridEnvironment;

/* The following warnings are useless, because this is an abstract base class
 * and we don't know if the user maybe want to use a variable or overwrite it. */
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace LIFE.Components.Agents.DalskiAgent.Agents {

  /// <summary>
  ///   A 2D-grid extension for the base agent.
  /// </summary>
  public abstract class GridAgent : Agent {

    private readonly IGridEnvironment<IGridCoordinate> _env; // IESC implementation for collision detection.
    private readonly GridPosition _position;                 // Agent position backing structure.
    protected readonly GridMover Mover;                      // Agent movement module.
    public readonly ReadOnlyGridPosition Position;           // Agent position container.


    /// <summary>
    ///   Create an agent for use in 2D grid-environments.
    /// </summary>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">Agent registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    /// <param name="env">Environment implementation.</param>
    /// <param name="id">The agent identifier (serialized GUID).</param>
    /// <param name="freq">MARS LIFE execution freqency.</param>
    protected GridAgent(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt,
                        IGridEnvironment<IGridCoordinate> env, byte[] id=null, int freq=1)
      : base(layer, regFkt, unregFkt, id, freq) {
      _env = env;
      _position = new GridPosition(0,0);
      Position = new ReadOnlyGridPosition(_position);
      Mover = new GridMover(env, _position, SensorArray);
    }


    /// <summary>
    ///   This function unbinds the agent from the environment.
    ///   It is triggered by the base agent, when alive flag is 'false'.
    /// </summary>
    protected override void Remove() {
      base.Remove();
      _env.Remove(_position);
    }


    /// <summary>
    ///   Return the result data for this agent.
    /// </summary>
    /// <returns>The agent's output values formatted into the result object.</returns>
    public AgentSimResult GetResultData() {
      return new AgentSimResult {
        AgentId = ID.ToString(),
        AgentType = GetType().Name,
        Layer = Layer.GetType().Name,
        Tick = GetTick(),
        Position = new [] {Position.X, Position.Y},
        Orientation = new [] {Position.Yaw, 0.0},
        AgentData = AgentData
      };
    }
  }
}