using CommonTypes.DataTypes;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitectureCommon.Interfaces;

namespace AgentTester.Wolves.Interactions {
  
  /// <summary>
  ///   Perform the movement of an agent.
  /// </summary>
  internal class MoveInteraction : IInteraction {
    private readonly Agent _agent; // The agent to move.
    private readonly Vector _newPosition; // The new (valid!) position.


    /// <summary>
    ///   Move an agent to a new position.
    /// </summary>
    /// <param name="agent">The agent to move.</param>
    /// <param name="newPosition">The new (valid!) position.</param>
    public MoveInteraction(Agent agent, Vector newPosition) {
      _agent = agent;
      _newPosition = newPosition;
    }


    /// <summary>
    ///   Execute the action. Set new position values.
    /// </summary>
    public void Execute() {
      _agent.Position = _newPosition;
    }
  }
}