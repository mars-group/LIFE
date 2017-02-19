using LIFE.Components.ESC.SpatialAPI.Entities.Movement;

namespace LIFE.Components.DalskiAgent.Perception {
  
  /// <summary>
  ///   Simple sensor for movement feedback.
  ///   It provides a 'MovementResult' in the sensor array.
  /// </summary>
  public class MovementSensor : ISensor {
    private MovementResult _movementResult;  // Current movement result.


    /// <summary>
    ///   Set a new movement result.
    /// </summary>
    /// <param name="movementResult">Result of (immediate) movement action.</param>
    internal void SetMovementResult(MovementResult movementResult) {
      _movementResult = movementResult;
    }
    

    /// <summary>
    ///   Sensor returns the movement result of last tick.
    /// </summary>
    /// <returns>Current movement result or 'null', if no movement occured.</returns>
    public object Sense() {
      var mr = _movementResult;
      _movementResult = null;  // Set 'null' to invalidate value for query in next tick.
      return mr;
    }
  }
}