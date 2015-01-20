using System.Collections.Generic;
using LifeAPI.Spatial;
using LifeAPI.Perception;

namespace LifeAPI.Environment {

    /// <summary>
    ///     The ESC should provide the possibility to check collisisions between entities and to explore defined areas.
    /// </summary>
    public interface IEnvironment : IDataSource {


        /// <summary>
        ///   Get the maximum extent (upper right position) of the environment. 
        /// </summary>
        TVector MaxDimension { get; }


        /// <summary>
        ///   Tells, if this environment is rasterized or not.
        /// </summary>
        bool IsGrid { get; }
        

        /// <summary>
        ///     Adds a new entity to the ESC at given position with given direction.
        /// </summary>
        /// <param name="entity">That should be added.</param>
        /// <param name="position">Absolute position, where the entity should be located.</param>
        /// <param name="rotation">The rotation defines a modification of the entities direction.</param>
        /// <returns>True, if the entity could be added at given position. False otherwise.</returns>
        bool Add(ISpatialEntity entity, TVector position, TVector rotation = default(TVector));

        /// <summary>
        ///     Adds a new entity to the ESC within the range of min and max.
        /// </summary>
        /// <param name="entity">That should be added.</param>
        /// <param name="min">The down, left position of an area that is spanned between min and max.</param>
        /// <param name="max">The upper, right position of an area that is spanned between min and max.</param>
        /// <param name="grid">If true, the random position will only contain integer.</param>
        /// <returns>True, if the entity could be added within range. False, otherwise.</returns>
        bool AddWithRandomPosition(ISpatialEntity entity, TVector min, TVector max, bool grid);

        //TODO auch mit zufälliger Rotation und ohne grid option


        /// <summary>
        ///     Removes given entity from the ESC.
        /// </summary>
        /// <param name="entity">That will be removed.</param>
        void Remove(ISpatialEntity entity);

        /// <summary>
        ///     Gives the entity a new geometry.
        /// </summary>
        /// <param name="entity">That should be resized.</param>
        /// <param name="shape">The new shape that should be assigned to the entity.</param>
        /// <returns>True, if the operation succeeded. False, otherwise (collision detection).</returns>
        bool Resize(ISpatialEntity entity, IShapeOld shape);

        /// <summary>
        ///     Tries to move given entity relatively from it's current position.
        /// </summary>
        /// <param name="entity">That will be moved.</param>
        /// <param name="movementVector">The vector defines a relative transition towards the goal.</param>
        /// <param name="rotation">The rotation defines a modification of the entities direction.</param>
        /// <returns>A MovementResult that defines the success of the operation and possible collisions.</returns>
        MovementResult Move(ISpatialEntity entity, TVector movementVector, TVector rotation = default(TVector));

        /// <summary>
        ///     Get spatial entities that corresponds with given geometry.
        /// </summary>
        /// <param name="spatial">Defines area that should be explored.</param>
        /// <returns>All spatial entities in geometry of the ESC.</returns>
        IEnumerable<ISpatialEntity> Explore(ISpatialObject spatial);

        /// <summary>
        ///     Get all added spatial entities of the ESC.
        /// </summary>
        /// <returns>All spatial entities of the ESC.</returns>
        IEnumerable<ISpatialEntity> ExploreAll();
    }

}