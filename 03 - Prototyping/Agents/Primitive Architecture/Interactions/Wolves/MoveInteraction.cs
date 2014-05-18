using Primitive_Architecture.Agents;
using Primitive_Architecture.Dummies;

namespace Primitive_Architecture.Interactions.Wolves {
  
  /// <summary>
  ///   Perform the movement of an agent.
  /// </summary>
  internal class MoveInteraction : Interaction {
    private readonly Agent _agent; // The agent to move.
    private readonly Vector _newPosition; // The new (valid!) position.


    /// <summary>
    ///   Move an agent to a new position.
    /// </summary>
    /// <param name="agent">The agent to move.</param>
    /// <param name="newPosition">The new (valid!) position.</param>
    public MoveInteraction(Agent agent, Vector newPosition) : base(null) {
      _agent = agent;
      _newPosition = newPosition;
    }


    //TODO These are not needed here ... concept change advised.
    public override bool CheckPreconditions() { return true; }
    public override bool CheckTrigger()       { return true; }


    /// <summary>
    ///   Execute the action. Set new position values.
    /// </summary>
    public override void Execute() {
      _agent.Position.X = _newPosition.X;
      _agent.Position.Y = _newPosition.Y;
    }
  }
}