using System;
using LIFE.API.GridCommon;
using LIFE.Components.Agents.DalskiAgent.Agents;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Shape;
using CT = LIFE.Components.ESC.SpatialAPI.Entities.Movement.CollisionType;

namespace LIFE.Components.Agents.DalskiAgent.Environment {

  /// <summary>
  ///   Internal spatial data represenation.
  ///   It also fullfills the ESC requirements.
  /// </summary>
  public class CartesianPosition : ISpatialEntity {

    private readonly Agent _agent;               // Agent associated with this position.
    private readonly CT _collisionType;          // Agent collision class.
    public double X => Shape.Position.X;         // Position (X-coordinate).
    public double Y => Shape.Position.Y;         // Position (Y-coordinate).
    public double Z => Shape.Position.Z;         // Position (Z-coordinate).
    public double Yaw => Shape.Rotation.Yaw;     // Agent orientation (X,Y).
    public double Pitch => Shape.Rotation.Pitch; // Agent climb angle.


    /// <summary>
    ///   Create a new cartesian position structure.
    /// </summary>
    /// <param name="agent">The agent this position belongs to.</param>
    /// <param name="collisionType">Agent collision type.</param>
    public CartesianPosition(Agent agent, string collisionType) {
      _agent = agent;
      Shape = new Cuboid(new Vector3(1,1,1), new Vector3(0,0,0));
      switch (collisionType.ToUpper()) {
        case "ENVIRONMENT": _collisionType = CT.StaticEnvironment; break;
        case "GHOST":       _collisionType = CT.Ghost; break;
        default:            _collisionType = CT.MassiveAgent; break;
      }
    }

    //_________________________________________________________________________
    // ISpatialEntity related methods for usage with IEnvironment instances.

    public IShape Shape { get; set; }            // A geometric shape describing this agent's body.
    public Guid AgentGuid => _agent.ID;          // Returns the base agent's identifier.
    public Type AgentType => _agent.GetType();   // Returns the agent's specific type.
    public Enum CollisionType => _collisionType; // Returns this agent's collision type.


    /// <summary>
    ///   Returns the current yaw as grid orientation constant.
    /// </summary>
    public GridDirection GridDirection {
      get {
        var yaw = Convert.ToInt32(Yaw);
        switch (yaw) {
          case   0: return GridDirection.Up;
          case  45: return GridDirection.UpRight;
          case  90: return GridDirection.Right;
          case 135: return GridDirection.DownRight;
          case 180: return GridDirection.Down;
          case 225: return GridDirection.DownLeft;
          case 270: return GridDirection.Left;
          case 315: return GridDirection.UpLeft;
          default : return GridDirection.Up;
        }
      }
    }
  }
}