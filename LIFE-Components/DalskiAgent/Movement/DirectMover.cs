using LIFE.Components.DalskiAgent.Interactions;
using LIFE.Components.DalskiAgent.Perception;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;

namespace LIFE.Components.DalskiAgent.Movement {

  /// <summary>
  ///   Enables basic agent movement by direct placement.
  /// </summary>
  public class DirectMover {

    private readonly IEnvironment _env;              // Environment interaction interface.
    private readonly AgentEntity _spatialData;       // Spatial data, needed for some calculations.
    private readonly MovementSensor _movementSensor; // Simple default sensor for movement feedback.


    /// <summary>
    ///   Create an agent mover for direct placement.
    /// </summary>
    /// <param name="env">Environment interaction interface.</param>
    /// <param name="spatialData">Spatial data, needed for some calculations.</param>
    /// <param name="movementSensor">Simple default sensor for movement feedback.</param>
    public DirectMover(IEnvironment env, AgentEntity spatialData, MovementSensor movementSensor) {
      _spatialData = spatialData;
      _movementSensor = movementSensor;
      _env = env;
    }


    /// <summary>
    ///   Set the agent to a new position.
    /// </summary>
    /// <param name="position">The target position.</param>
    /// <param name="dir">The new direction [default: use old heading].</param> 
    /// <returns>An interaction object that contains the code to execute this movement.</returns>
    public IInteraction SetToPosition(Vector3 position, Direction dir = null) {
      if (dir == null) dir = _spatialData.Direction;
      var vector = new Vector3(
        position.X - _spatialData.Position.X,  
        position.Y - _spatialData.Position.Y,  
        position.Z - _spatialData.Position.Z  
      );

      return new MovementAction(delegate {
        var result = _env.Move(_spatialData, vector, dir);
        _movementSensor.SetMovementResult(result);
      });
    }
  }
}