﻿using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Dummies;
using GenericAgentArchitecture.Interactions;

namespace AgentTester.Wolves.Interactions {
  
  /// <summary>
  ///   Perform the movement of an agent.
  /// </summary>
  internal class MoveInteraction : Interaction {
    private readonly Agent _agent; // The agent to move.
    private readonly Float3 _newPosition; // The new (valid!) position.


    /// <summary>
    ///   Move an agent to a new position.
    /// </summary>
    /// <param name="agent">The agent to move.</param>
    /// <param name="newPosition">The new (valid!) position.</param>
    public MoveInteraction(Agent agent, Float3 newPosition) : base(null) {
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
      _agent.Position.Center = _newPosition;
    }
  }
}