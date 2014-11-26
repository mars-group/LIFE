using System;
using DalskiAgent.Movement.Movers;
using GenericAgentArchitectureCommon.Datatypes;
using GenericAgentArchitectureCommon.Interfaces;

namespace DalskiAgent.Movement.Actions {

  /// <summary>
  ///   This class allows to set movement and turning speeds to control the agent.
  /// </summary>
  public class ContinuousMovementAction : IInteraction {

    private readonly ContinuousMover _mover; // The agent movement module.
    private readonly Direction _targetDir;   // The target direction (mode 1).
    private readonly double _speed;           // Movement speed.
    private readonly float _pitchAS;         // Pitch turning speed (mode 2).
    private readonly float _yawAS;           // Yaw turning speed (mode 2).
    private readonly bool _turnSpdUsed;      // 'True' on mode 2, otherwise 'false'.


    /// <summary>
    ///   Create a movement action for continuous environments (private base constructor).
    /// </summary>
    /// <param name="mover">The agent movement module.</param>
    /// <param name="speed">The new movement speed.</param>
    private ContinuousMovementAction(AgentMover mover, double speed) {
      if (mover is ContinuousMover) _mover = (ContinuousMover) mover;
      else throw new Exception("[ContinuousMovementAction] Error: Agent movement type is incompatible!");
      _speed = speed;
    }


    /// <summary>
    ///   Create a movement action for continuous environments (direction set version).
    /// </summary>
    /// <param name="mover">The agent movement module.</param>
    /// <param name="speed">The new movement speed.</param>
    /// <param name="dir">The direction the agent is facing.</param>
    public ContinuousMovementAction(AgentMover mover, double speed, Direction dir) : this(mover, speed) {
      _targetDir = dir;
      _turnSpdUsed = false;
    }


    /// <summary>
    ///   Create a movement action for continuous environments (turning speed version).
    /// </summary>
    /// <param name="mover">The agent movement module.</param>
    /// <param name="speed">The new movement speed.</param>
    /// <param name="pitchAS">New vertical turning speed.</param>
    /// <param name="yawAS">New horizontal turning speed.</param>
    public ContinuousMovementAction(AgentMover mover, float speed, float pitchAS, float yawAS) : this(mover, speed) {
      _pitchAS = pitchAS;
      _yawAS = yawAS;
      _turnSpdUsed = true;
    }


    /// <summary>
    ///   Call agent mover. Signature depends on whether angular speeds or direction mode is used.
    /// </summary>
    public void Execute() {
      if (_turnSpdUsed) _mover.Move(_speed, _pitchAS, _yawAS);
      else _mover.Move(_speed, _targetDir);
    }
  }
}
