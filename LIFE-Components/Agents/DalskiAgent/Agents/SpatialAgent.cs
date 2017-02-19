using System;
using System.Collections.Generic;
using LIFE.API.Layer;
using LIFE.API.Results;
using LIFE.Components.DalskiAgent.Movement;
using LIFE.Components.ESC.SpatialAPI.Entities.Movement;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Shape;
using MongoDB.Driver.GeoJsonObjectModel;

namespace LIFE.Components.DalskiAgent.Agents {

  /// <summary>
  ///   This agent is part of a spatial world. It has a position and 
  ///   is registered at an environment to provide collision detection. 
  ///   Per default it is immobile but it can be equipped with a movement module.
  /// </summary>
  public abstract class SpatialAgent : Agent {

    private IEnvironment _env;      // IESC implementation for collision detection.
    private readonly ILayer _layer; // Layer reference, needed for type/tick in result object.


    /// <summary>
    ///   Class for agent movement.
    /// </summary>
    protected AgentMover Mover;


    /// <summary>
    ///   R/O spatial data container.
    /// </summary>
    public readonly IAgentEntity SpatialData;


    /// <summary>
    ///   Dictionary for arbitrary values. It is passed to the result database.
    /// </summary>
    protected readonly Dictionary<string, object> AgentData;


    /// <summary>
    ///   Instantiate a new agent with spatial data. Only available for specializations.
    /// </summary>
    /// <param name="layer">Layer reference needed for delegate calls.</param>
    /// <param name="regFkt">Agent registration function pointer.</param>
    /// <param name="unregFkt"> Delegate for unregistration function.</param>
    /// <param name="env">Environment implementation reference.</param>
    /// <param name="id">Fixed GUID to use in this agent (optional).</param>
    /// <param name="shape">Shape describing this agent's body and initial parameters.</param>
    /// <param name="minPos">Minimum position for random placement [default: origin].</param>
    /// <param name="maxPos">Maximal random position [default: environmental extent].</param>
    /// <param name="collisionType">Agent collision type [default: 'MassiveAgent'].</param>
    /// <param name="executionGroup">
    ///   The execution group of your agent:
    ///   0 : execute never
    ///   1 : execute every tick (default)
    ///   n : execute every tick where tick % executionGroup == 0
    /// </param>
    protected SpatialAgent(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, IEnvironment env,
      byte[] id = null, IShape shape = null, Vector3 minPos = default(Vector3), Vector3 maxPos = default(Vector3),
      Enum collisionType = null, int executionGroup = 1) : base(layer, regFkt, unregFkt, id, executionGroup) {

        // Set up the agent entity. Per default it is collidable.
        var entity = new AgentEntity();
        if (collisionType != null) entity.CollisionType = collisionType;
        else entity.CollisionType = CollisionType.MassiveAgent;

        entity.AgentGuid = ID;        // Use this agent' ID also in the spatial entity.
        entity.AgentType = GetType(); // Set the class type of this agent (specific implementation).
        SpatialData = entity;         // Set entity object to generic IAgentEntity query reference.


        // Set agent shape. If the agent has no shape yet, create a cube facing north and add at a random position.
        if (shape != null) entity.Shape = shape;
        else entity.Shape = new Cuboid(new Vector3(1.0, 1.0, 1.0), new Vector3(), new Direction());

        // Place the agent in the environment.
        bool success;
        if (shape != null && minPos.IsNull() && maxPos.IsNull()) {
          success = env.Add(entity, entity.Shape.Position, entity.Shape.Rotation);
        }
        else {
          // Random position shall be used.
          if (minPos.IsNull()) minPos = Vector3.Zero;
          if (maxPos.IsNull()) maxPos = env.MaxDimension;
          success = env.AddWithRandomPosition(entity, minPos, maxPos, env.IsGrid);
        }

        if (!success)
          throw new Exception("[SpatialAgent] Agent placement in environment failed (ESC returned 'false')!");

        // Save the environment reference and add an agent mover.
        _env = env;
        _layer = layer;
        Mover = new AgentMover(_env, entity, SensorArray);
        AgentData = new Dictionary<string, object>();
    }


    /// <summary>
    ///   This function unbinds the agent from the environment.
    ///   It is triggered by the base agent, when alive flag is 'false'.
    /// </summary>
    protected override void Remove() {
      base.Remove();
      _env.Remove((AgentEntity)SpatialData);
    }


    /// <summary>
    ///   Set an arbitrary information string to the underlying agent entity.
    /// </summary>
    /// <param name="info">Information string.</param>
    protected void SetAgentEntityInformation(string info) {
      ((AgentEntity) SpatialData).AgentInformation = info;      
    }


    /// <summary>
    ///   Moves the agent to a new environment reference. 
    /// </summary>
    /// <param name="newEnv">The new environment to use.</param>
    /// <param name="newPos">Position to insert to.</param>
    /// <returns>Placement result of new environment.</returns>
    protected bool ChangeEnvironment(IEnvironment newEnv, Vector3 newPos) {
      _env.Remove((AgentEntity) SpatialData);
      _env = newEnv;
      Mover = new AgentMover(_env, (AgentEntity)SpatialData, SensorArray);
      return _env.Add((AgentEntity)SpatialData, newPos);
    }


    /// <summary>
    ///   Returns the visualization object for this agent.
    /// </summary>
    /// <returns>The agent's output values formatted into the result object.</returns>
    public virtual AgentSimResult GetResultData() {
      var pos = SpatialData.Position;
      var dir = SpatialData.Direction;
      return new AgentSimResult {
        AgentId = ID.ToString(),
        AgentType = GetType().Name,
        Layer = _layer.GetType().Name,
        Tick = _layer.GetCurrentTick(),
        Position = GeoJson.Point(new GeoJson2DCoordinates(pos.X, pos.Y)),
        Orientation = new[] { dir.Yaw, dir.Pitch, 0.0f},
        AgentData = AgentData
      };
    }
  }
}