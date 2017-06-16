using System;
using System.Collections.Generic;
using EnvironmentServiceComponent.SpatialAPI.Entities.Movement;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Movement;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace LIFE.Components.ESC.SpatialAPI.Environment
{
    /// <summary>
    ///     Is called after commit to publish the operation results.
    ///     In case this is called after an UNSUCCESSFUL environment.Add (ResultCode != OK) newPosition will contain the Guid of the agent that could'nt be added.
    /// 
    /// </summary>
    /// <param name="result">Provides information about the operations success and otherwise the collisions entities.</param>
    /// <param name="newPosition">Contains the new Position if the action was successful</param>
    public delegate void MovementDelegate(EnvironmentResult result, ISpatialEntity newPosition);

    /// <summary>
    ///     Is called after commit to publish the operation results.
    /// </summary>
    /// <param name="result">Provides a list of explored entities and the resultcode.</param>
    public delegate void ExploreDelegate(EnvironmentResult result);





    /// <summary>
    ///     The IAsyncEnvironmentRefactor should provide the possibility to check collisions between entities and to explore defined
    ///     areas. It is asynchronous thus provides the results after commit.
    /// </summary>
    public interface IAsyncEnvironment
    {

        /// <summary>
        ///     Returns the SpatialEntity that corresponds to the guid -> null if it does not exist.
        /// </summary>
        /// <param name="entity">That should be added.</param>
        ISpatialEntity GetSpatialEntity(Guid agentID);

        /// <summary>
        ///     Get the maximum extent (upper right position) of the environment.
        /// </summary>
        Vector3 MaxDimension { get; set; }

        /// <summary>
        ///     Tells, if this environment is rasterized or not.
        /// </summary>
        bool IsGrid { get; set; }

        /// <summary>
        ///     Adds a new entity to the ESC at given position with given direction.
        /// </summary>
        /// <param name="entity">That should be added.</param>
        /// <param name="movementDelegate">Signalizes the termination of the operation and indicates its success.</param>
        //void Add(ISpatialEntity entity, MovementDelegate movementDelegate);
        void Add(ISpatialEntity entity, MovementDelegate movementDelegate);

        /// <summary>
        ///     Adds a new entity to the ESC within the range of min and max.
        /// </summary>
        /// <param name="entity">That should be added.</param>
        /// <param name="min">The down, left position of an area that is spanned between min and max.</param>
        /// <param name="max">The upper, right position of an area that is spanned between min and max.</param>
        /// <param name="grid">If true, the random position will only contain integer.</param>
        /// <param name="movementDelegate">Signalizes the termination of the operation and indicates its success.</param>
        void AddWithRandomPosition(ISpatialEntity entity, Vector3 min, Vector3 max, bool grid, MovementDelegate movementDelegate);

        /// <summary>
        ///     Removes given entity from the ESC. This operation succeeds always.
        /// </summary>
        /// <param name="agentId">That will be removed.</param>
        /// <param name="removeDelegate">That will be called when the Agent got removed and inserts its endposition</param>
        void Remove(Guid agentId, Action<ISpatialEntity> removeDelegate);

        /// <summary>
        ///     Gives the entity a new shape but remains on previous position. It is possible, that the new shape has to be
        ///     transformed in order to remain on previous position.
        /// </summary>
        /// <param name="agentId">That should be resized.</param>
        /// <param name="shape">The new shape that should be assigned to the entity.</param>
        /// <param name="movementDelegate">Signalizes the termination of the operation and indicates its success.</param>
        void Resize(Guid agentId, IShape shape, MovementDelegate movementDelegate);

        /// <summary>
        ///     Tries to move given entity relatively from it's current position.
        /// </summary>
        /// <param name="agentId">That will be moved.</param>
        /// <param name="movementVector">The vector defines a relative transition towards the goal.</param>
        /// <param name="rotation">
        ///     The rotation defines the absolute orientation of the entity. If null, the previous rotation is used but
        ///     not modified.
        /// </param>
        /// <param name="movementDelegate">
        ///     Signalizes the termination of the operation and indicates its success and possible
        ///     collisions.
        /// </param>

        void Move(Guid agentId, Vector3 movementVector, Direction rotation, MovementDelegate movementDelegate, int maxResults = 100);

        /// <summary>
        ///     Get spatial entities that corresponds with given shape and collision type.
        /// </summary>
        /// <param name="shape">Defines geometry that should be explored.</param>
        /// <param name="exploreDelegate">Signalizes the termination of the operation and provides a list of found entities.</param>
        /// <param name="agentType">If agent type is not null, only ISpatialEntities with same agent type are returned.</param>
        /// </param>
        /// 
        void Explore(IShape shape, ExploreDelegate exploreDelegate, Type agentType = null,  int maxResults = 100);

        /// <summary>
        ///     Get all added spatial entities of the ESC.
        /// </summary>
        /// <param name="exploreDelegate">Signalizes the termination of the operation and provides a list of found entities.</param>
        /// 
        /// <returns>All spatial entities of the ESC.</returns>
        void ExploreAll(ExploreDelegate exploreDelegate);

        /// <summary>
        ///     Executes all operations that were called since the last commit. Triggers all delegates afterwards. Must be called
        ///     in the PostTick-phase of the ActiveSteppedLayer.
        /// </summary>
        void Commit();
    }
}
