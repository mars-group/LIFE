using System;
using LIFE.Components.DalskiAgent.Interactions;
using LIFE.Components.DalskiAgent.Perception;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;

namespace LIFE.Components.DalskiAgent.Movement {
  
  /// <summary>
  ///   Movement module with speeds and position calculation.
  /// </summary>
  public class ContinuousMover {

    private readonly IEnvironment _env;              // Environment interaction interface.
    private readonly AgentEntity _spatialData;       // Spatial data, needed for some calculations.
    private readonly MovementSensor _movementSensor; // Simple default sensor for movement feedback.
    

    /// <summary>
    ///   Create a continuous agent mover.
    /// </summary>
    /// <param name="env">Environment interaction interface.</param>
    /// <param name="spatialData">Spatial data, needed for some calculations.</param>
    /// <param name="movementSensor">Simple default sensor for movement feedback.</param>
    public ContinuousMover(IEnvironment env, AgentEntity spatialData, MovementSensor movementSensor) {
      _spatialData = spatialData;
      _movementSensor = movementSensor;
      _env = env;
    }


    /// <summary>
    ///   Moves the agent. This version uses turning speeds.
    /// </summary>
    /// <param name="speed">The movement speed.</param>
    /// <param name="pitchAs">Pitch changing angular speed.</param>
    /// <param name="yawAs">Rotary speed (vertical axis).</param>
    /// <returns>An interaction object that contains the code to execute this movement.</returns>  
    public IInteraction Move(double speed, float pitchAs, float yawAs) {
      var dir = new Direction();
      dir.SetPitch(_spatialData.Direction.Pitch + pitchAs);
      dir.SetYaw(_spatialData.Direction.Yaw + yawAs);
      return Move(speed, dir);
    }


    /// <summary>
    ///   Moves the agent with some speed in a given direction. 
    /// </summary>
    /// <param name="speed">Movement speed.</param>
    /// <param name="dir">The direction [default: Use old heading].</param>
    /// <returns>An interaction object that contains the code to execute this movement.</returns> 
    public IInteraction Move(double speed, Direction dir = null) {
      if (dir == null) dir = _spatialData.Direction;
      
      // Calculate target position based on current position, heading and speed.     
      var pitchRad = Direction.DegToRad(dir.Pitch);
      var yawRad = Direction.DegToRad(dir.Yaw);
      var factor = speed;
      var x = factor * Math.Cos(pitchRad) * Math.Sin(yawRad);
      var y = factor * Math.Sin(pitchRad); 
      var z = factor * Math.Cos(pitchRad) * Math.Cos(yawRad);
      var vector = new Vector3(x, y, z);

      return new MovementAction(delegate {
        var result = _env.Move(_spatialData, vector, dir);
        _movementSensor.SetMovementResult(result);
      });
    }


    /// <summary>
    ///   This function automatically sets the reference, speed, yaw and pitch 
    ///   values to go to the supplied point. It then returns an appropriate action. 
    /// </summary>
    /// <param name="speed">The agent's movement speed.</param>
    /// <param name="targetPos">A point the agent shall go to.</param>
    /// <returns>A movement action, ready for execution. </returns>
    public IInteraction MoveTowardsPosition(double speed, Vector3 targetPos) {

      // Check, if we are already there. Otherwise no need to move anyway.
      var distance = _spatialData.Position.GetDistance(targetPos);
      if (Math.Abs(distance) <= float.Epsilon) return null;

      // Get the right direction.
      var diff = new Vector3(targetPos.X - _spatialData.Position.X, 
                             targetPos.Y - _spatialData.Position.Y,
                             targetPos.Z - _spatialData.Position.Z);
      var dir = new Direction();
      dir.SetDirectionalVector(diff);

      // Check the speed. If we would go too far, reduce it accordingly.
      if (distance < speed) speed = distance;
  
      // Save calculated values to new movement class and return.
      return Move(speed, dir);
    }


    /// <summary>
    ///   Calculate the needed direction towards a given position.
    /// </summary>
    /// <param name="target">The target to get orientation to.</param>
    /// <returns>The yaw (corrected to 0 - lt. 360). </returns>
    public Direction CalculateDirectionToTarget(Vector3 target) {
      var diff = new Vector3(target.X - _spatialData.Position.X, 
                             target.Y - _spatialData.Position.Y,
                             target.Z - _spatialData.Position.Z);
      
      // Create new direction, set joint vector as reference and return.
      var dir = new Direction();
      dir.SetDirectionalVector(diff);
      return dir;
    }
  }
}