using System;
using System.Collections.Generic;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Movement;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace LIFE.Components.ESC.SpatialAPI.Environment {
  
  /// <summary>
  ///   Is called after commit to publish the operation results.
  /// </summary>
  /// <param name="result">Provides information about the operations success and otherwise the collisions entities.</param>
  public delegate void MovementDelegate(MovementResult result);

  /// <summary>
  ///   Is called after commit to publish the operation results.
  /// </summary>
  /// <param name="result">Provides a list of explored entities.</param>
  public delegate void ExploreDelegate(IEnumerable<ISpatialEntity> result);

  /// <summary>
  ///   The IAsyncEnvironment should provide the possibility to check collisions between entities and to explore defined
  ///   areas. It is asynchronous thus provides the results after commit.
  /// </summary>
  public interface IAsyncEnvironment {
    /// <summary>
    ///   Adds a new entity to the ESC at given position with given direction.
    /// </summary>
    /// <param name="entity">That should be added.</param>
    /// <param name="position">Absolute position, where the entity should be located.</param>
    /// <param name="rotation">
    ///   The rotation defines the absolute orientation of the entity. If null, the standard orientation is used.
    /// </param>
    /// <param name="movementDelegate">Signalizes the termination of the operation and indicates its success.</param>
    void Add(ISpatialEntity entity, Vector3 position, Direction rotation, MovementDelegate movementDelegate);

    /// <summary>
    ///   Adds a new entity to the ESC within the range of min and max.
    /// </summary>
    /// <param name="entity">That should be added.</param>
    /// <param name="min">The down, left position of an area that is spanned between min and max.</param>
    /// <param name="max">The upper, right position of an area that is spanned between min and max.</param>
    /// <param name="grid">If true, the random position will only contain integer.</param>
    /// <param name="movementDelegate">Signalizes the termination of the operation and indicates its success.</param>
    void AddWithRandomPosition(ISpatialEntity entity, Vector3 min, Vector3 max, bool grid,
      MovementDelegate movementDelegate);

    /// <summary>
    ///   Removes given entity from the ESC. This operation succeeds always.
    /// </summary>
    /// <param name="entity">That will be removed.</param>
    void Remove(ISpatialEntity entity);

    /// <summary>
    ///   Gives the entity a new shape but remains on previous position. It is possible, that the new shape has to be
    ///   transformed in order to remain on previous position.
    /// </summary>
    /// <param name="entity">That should be resized.</param>
    /// <param name="shape">The new shape that should be assigned to the entity.</param>
    /// <param name="movementDelegate">Signalizes the termination of the operation and indicates its success.</param>
    void Resize(ISpatialEntity entity, IShape shape, MovementDelegate movementDelegate);

    /// <summary>
    ///   Tries to move given entity relatively from it's current position.
    /// </summary>
    /// <param name="entity">That will be moved.</param>
    /// <param name="movementVector">The vector defines a relative transition towards the goal.</param>
    /// <param name="rotation">
    ///   The rotation defines the absolute orientation of the entity. If null, the previous rotation is used but
    ///   not modified.
    /// </param>
    /// <param name="movementDelegate">
    ///   Signalizes the termination of the operation and indicates its success and possible
    ///   collisions.
    /// </param>
    void Move(ISpatialEntity entity, Vector3 movementVector, Direction rotation, MovementDelegate movementDelegate);

    /// <summary>
    ///   Get spatial entities that corresponds with given parameters. Beware that geometry and collision type are both taken
    ///   into account. So if you ask for a specific agent type that does not correspond with given collision type, it will
    ///   not be found. The order of reduction is: shape, collision type, agent type.
    /// </summary>
    /// <param name="spatial">Defines geometry and collision type that should restrict the exploration.</param>
    /// <param name="exploreDelegate">Signalizes the termination of the operation and provides a list of found entities.</param>
    /// <param name="agentType">If agent type is not null, only ISpatialEntities with same agent type are returned.</param>
    void Explore(ISpatialObject spatial, ExploreDelegate exploreDelegate, Type agentType = null);

    /// <summary>
    ///   Get spatial entities that corresponds with given shape and collision type.
    /// </summary>
    /// <param name="shape">Defines geometry that should be explored.</param>
    /// <param name="exploreDelegate">Signalizes the termination of the operation and provides a list of found entities.</param>
    /// <param name="collisionType">
    ///   If collision type is not null, only ISpatialEntities with matching collision type are
    ///   returned. If it is null, all ISpatialEntities intersecting given shape are returend.
    /// </param>
    void Explore(IShape shape, ExploreDelegate exploreDelegate, Enum collisionType = null);

    /// <summary>
    ///   Get all added spatial entities of the ESC.
    /// </summary>
    /// <returns>All spatial entities of the ESC.</returns>
    IEnumerable<ISpatialEntity> ExploreAll();

    /// <summary>
    ///   Executes all operations that were called since the last commit. Triggers all delegates afterwards. Must be called
    ///   in the PostTick-phase of the ActiveSteppedLayer.
    /// </summary>
    void Commit();
  }
}