using System;
using System.Collections.Generic;
using LIFE.API.LIFECapabilities;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Movement;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace LIFE.Components.ESC.SpatialAPI.Environment {

  /// <summary>
  ///   The ESC should provide the possibility to check collisisions between entities and to explore defined areas.
  /// </summary>
  public interface IESC : ILifeAutoInitialized {
  
    /// <summary>
    ///   Get the maximum extent (upper right position) of the environment.
    /// </summary>
    Vector3 MaxDimension { get; set; }

    /// <summary>
    ///   Tells, if this environment is rasterized or not.
    /// </summary>
    bool IsGrid { get; set; }

    /// <summary>
    ///   Adds a new entity to the ESC at given position with given direction.
    /// </summary>
    /// <param name="entity">That should be added.</param>
    /// <param name="position">Absolute position, where the entity should be located.</param>
    /// <param name="rotation">
    ///   The rotation defines the absolute orientation of the entity. If null, the standard orientation is used.
    /// </param>
    /// <returns>True, if the entity could be added at given position. False otherwise.</returns>
    bool Add(ISpatialEntity entity, Vector3 position, Direction rotation = null);

    /// <summary>
    ///   Adds a new entity to the ESC within the range of min and max.
    /// </summary>
    /// <param name="entity">That should be added.</param>
    /// <param name="min">The down, left position of an area that is spanned between min and max.</param>
    /// <param name="max">The upper, right position of an area that is spanned between min and max.</param>
    /// <param name="grid">If true, the random position will only contain integer.</param>
    /// <returns>True, if the entity could be added within range. False, otherwise.</returns>
    bool AddWithRandomPosition(ISpatialEntity entity, Vector3 min, Vector3 max, bool grid);

    /// <summary>
    ///   Removes given entity from the ESC.
    /// </summary>
    /// <param name="entity">That will be removed.</param>
    void Remove(ISpatialEntity entity);

    /// <summary>
    ///   Gives the entity a new shape but remains on previous position. It is possible, that the new shape has to be
    ///   transformed in order to remain on previous position.
    /// </summary>
    /// <param name="entity">That should be resized.</param>
    /// <param name="shape">The new shape that should be assigned to the entity.</param>
    /// <returns>True, if the operation succeeded. False, otherwise (collision detection).</returns>
    bool Resize(ISpatialEntity entity, IShape shape);

    /// <summary>
    ///   Tries to move given entity relatively from it's current position.
    /// </summary>
    /// <param name="entity">That will be moved.</param>
    /// <param name="movementVector">The vector defines a relative transition towards the goal.</param>
    /// <param name="rotation">
    ///   The rotation defines the absolute orientation of the entity. If null, the previous rotation is used but
    ///   not modified.
    /// </param>
    /// <returns>A MovementResult that defines the success of the operation and possible collisions.</returns>
    MovementResult Move(ISpatialEntity entity, Vector3 movementVector, Direction rotation = null);


    IEnumerable<ISpatialEntity> Explore(Sphere sphere, int maxResults = -1);

    /// <summary>
    ///   Get spatial entities that corresponds with given parameters. Beware that geometry and collision type are both taken
    ///   into account. So if you ask for a specific agent type that does not correspond with given collision type, it will
    ///   not be found. The order of reduction is: shape, collision type, agent type.
    /// </summary>
    /// <param name="spatial">Defines geometry and collision type that should restrict the exploration.</param>
    /// <param name="agentType">If agent type is not null, only ISpatialEntities with same agent type are returned.</param>
    /// <returns>All spatial entities that correspond with given parameters.</returns>
    IEnumerable<ISpatialEntity> Explore(ISpatialObject spatial, Type agentType = null);

    /// <summary>
    ///   Get spatial entities that corresponds with given shape and collision type.
    /// </summary>
    /// <param name="shape">Defines geometry that should be explored.</param>
    /// <param name="collisionType">
    ///   If collision type is not null, only ISpatialEntities with matching collision type are
    ///   returned. If it is null, all ISpatialEntities intersecting given shape are returend.
    /// </param>
    /// <returns>All spatial entities that correspond with given parameters</returns>
    IEnumerable<ISpatialEntity> Explore(IShape shape, Enum collisionType = null);

    /// <summary>
    ///   Get all added spatial entities of the ESC.
    /// </summary>
    /// <returns>All spatial entities of the ESC.</returns>
    IEnumerable<ISpatialEntity> ExploreAll();
  }
}