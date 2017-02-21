using LIFE.Components.Agents.DalskiAgent.Perception;
using LIFE.Components.ESC.SpatialAPI.Environment;

namespace LIFE.Components.Agents.DalskiAgent.Movement {

  /// <summary>
  ///   This mover class serves as a base for agent movement.
  ///   It uses an IEnvironment implementation to move the agent.
  /// </summary>
  public class AgentMover {

   
    /// <summary>
    ///   Module for grid-style movement.
    /// </summary>
    public readonly GridMover Grid;
    
    /// <summary>
    ///   Movement module with speeds and position calculation.
    /// </summary>
    public readonly ContinuousMover Continuous;
    
    /// <summary>
    ///   Enables basic agent movement by direct placement.
    /// </summary>
    public readonly DirectMover Direct;
    
    /// <summary>
    ///   Movement module for applications with GPS coordinates.
    /// </summary>
    public readonly GPSMover GPS;


    /// <summary>
    ///   Instantiate a new agent mover.
    /// </summary>
    /// <param name="env">IEnvironment implementation to use.</param>
    /// <param name="spatialData">Spatial data, needed for some calculations.</param>
    /// <param name="sensorArray">Sensor and perception storage (used for movement results).</param>
    public AgentMover(IEnvironment env, AgentEntity spatialData, SensorArray sensorArray) {
      var movementSensor = new MovementSensor();
      sensorArray.AddSensor(movementSensor);
      Direct = new DirectMover(env, spatialData, movementSensor);
      Continuous = new ContinuousMover(env, spatialData, movementSensor);
      Grid = new GridMover(env, spatialData, movementSensor);
      GPS = new GPSMover(env, spatialData, movementSensor);
    }
  }
}