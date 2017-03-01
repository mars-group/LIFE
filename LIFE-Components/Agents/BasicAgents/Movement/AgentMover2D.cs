using LIFE.Components.Agents.BasicAgents.Reasoning;

namespace LIFE.Components.Agents.BasicAgents.Movement {

  /// <summary>
  ///   Two-dimensional, continuous movement module.
  ///   This is just a wrapper for the AgentMover3D, providing a 2D interface!
  /// </summary>
  public class AgentMover2D {

    private readonly AgentMover3D _mover3D; // The 3D mover to use internally.


    /// <summary>
    ///   Create a new 2D mover for continuous environments.
    /// </summary>
    /// <param name="mover3D">The 3D mover to use internally.</param>
    public AgentMover2D(AgentMover3D mover3D) {
      _mover3D = mover3D;
    }


    /// <summary>
    ///   Try to insert this agent into the environment at the given position.
    /// </summary>
    /// <param name="x">Agent start position (x-coordinate).</param>
    /// <param name="y">Agent start position (y-coordinate).</param>
    /// <returns>Success flag. If failed, the agent may not be moved!</returns>
    public bool InsertIntoEnvironment(double x, double y) {
      return _mover3D.InsertIntoEnvironment(x, y, 0);
    }


    /// <summary>
    ///   MoveToPosition the agent forward with a given speed.
    /// </summary>
    /// <param name="distance">The distance to move.</param>
    /// <returns>Interaction expressing this movement.</returns>
    public MovementAction MoveForward(double distance) {
      return _mover3D.MoveForward(distance);
    }


    /// <summary>
    ///   MoveToPosition the agent into a direction.
    /// </summary>
    /// <param name="distance">The distance to move.</param>
    /// <param name="yaw">New agent heading.</param>
    /// <returns>An interaction describing the movement.</returns>
    public MovementAction MoveInDirection(double distance, double yaw) {
      return _mover3D.MoveInDirection(distance, yaw, 0);
    }


    /// <summary>
    ///   MoveToPosition the agent to a position.
    /// </summary>
    /// <param name="distance">The distance to move.</param>
    /// <param name="x">X-coordinate target position.</param>
    /// <param name="y">Y-coordinate of target position.</param>
    /// <returns>An interaction describing the movement.</returns>
    public MovementAction MoveTowardsPosition(double distance, double x, double y) {
      return _mover3D.MoveTowardsPosition(distance, x, y, 0);
    }


    /// <summary>
    ///   Set the agent to a new position.
    /// </summary>
    /// <param name="x">X-coordinate target position.</param>
    /// <param name="y">Y-coordinate of target position.</param>
    /// <returns>An interaction describing the movement.</returns>
    public MovementAction SetToPosition(double x, double y) {
      return _mover3D.SetToPosition(x, y, 0);
    }
  }
}