using System;
using System.Collections.Generic;
using System.Linq;
using EnvironmentServiceComponent.Entities;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Movement;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Environment;
using SpatialAPI.Shape;
using SpatialObjectOctree.Implementation;
using SpatialObjectOctree.Interface;

namespace EnvironmentServiceComponent.Implementation {

    /// <summary>
    ///     3-dimensional implementation of the IEnvironment interface.
    /// </summary>
    public class EnvironmentServiceComponent : IEnvironment {
        private const int MaxAttempsToAddRandom = 100;
        private readonly bool[,] _collisionMatrix;
        private readonly IOctree<ISpatialEntity> _octree;
        private readonly Random _random = new Random();
        private readonly object _syncLock = new object();

        /// <summary>
        ///     Initializes the environemt.
        /// </summary>
        /// <param name="collisionMatrix">Defines the collision behaviour of added ISpatialEntities.</param>
        public EnvironmentServiceComponent(bool[,] collisionMatrix = null) {
            _collisionMatrix = collisionMatrix ?? CollisionMatrix.Get(); // if null use standard behaviour instead
            _octree = new SpatialObjectOctree<ISpatialEntity>(new Vector3(25, 25, 25), 1, true);
        }

        /// <summary>
        ///     Tries to move given entity relatively from it's current position.
        /// </summary>
        /// <param name="entityIsAdded">Indicates if the entity is already in the system.</param>
        /// <param name="entity">That will be moved.</param>
        /// <param name="movementVector">The vector defines a relative transition towards the goal.</param>
        /// <param name="rotation">
        ///     The rotation defines the absolute orientation of the entity. If null, the previous rotation is used but
        ///     not modified.
        /// </param>
        /// <returns>A MovementResult that defines the success of the operation and possible collisions.</returns>
        private MovementResult Move
            (bool entityIsAdded, ISpatialEntity entity, Vector3 movementVector, Direction rotation = null) {
            rotation = rotation ?? new Direction();
            IShape newShape = entity.Shape.Transform(movementVector, rotation);
            lock (_syncLock) {
                List<ISpatialEntity> collisions = Explore(newShape, entity.CollisionType).ToList();
                collisions.Remove(entity);

                if (collisions.Any()) {
                    return new MovementResult(collisions);
                }
                if (entityIsAdded) {
                    _octree.Remove(entity);
                }
                entity.Shape = newShape;
                _octree.Insert(entity);
                return new MovementResult();
            }
        }

        /// <summary>
        ///     Indicates if the combination of collision types causes a collision. Respect the order.
        /// </summary>
        /// <param name="givenCollisionType">The collision type that may collide with the other.</param>
        /// <param name="foundCollisionType">The collision type of which the correspondency is checked.</param>
        /// <returns>True, if the combination results in a collision, false otherwise.</returns>
        private bool Collides(int givenCollisionType, int foundCollisionType) {
            return _collisionMatrix[givenCollisionType, foundCollisionType];
        }

        /// <summary>
        ///     Generates a random position within the given bounds.
        /// </summary>
        /// <param name="min">Lower boundary.</param>
        /// <param name="max">Upper boundary.</param>
        /// <param name="grid">Defines, if position only holds integer values.</param>
        /// <returns>The generated position.</returns>
        private Vector3 GenerateRandomPosition(Vector3 min, Vector3 max, bool grid) {
            if (grid) {
                int x = _random.Next((int) min.X, (int) max.X + 1);
                int y = _random.Next((int) min.Y, (int) max.Y + 1);
                int z = _random.Next((int) min.Z, (int) max.Z + 1);
                return new Vector3(x, y, z);
            }
            else {
                double x = GetRandomNumber(min.X, max.X);
                double y = GetRandomNumber(min.Y, max.Y);
                double z = GetRandomNumber(min.Z, max.Z);
                return new Vector3(x, y, z);
            }
        }

        /// <summary>
        ///     Provides a random number within the given range.
        /// </summary>
        /// <param name="min">Lower boundary.</param>
        /// <param name="max">Upper boundary.</param>
        /// <returns>The generated random number.</returns>
        private double GetRandomNumber(double min, double max) {
            return _random.NextDouble()*(max - min) + min;
        }

        /// <summary>
        ///     Get spatial entities that corresponds with given parameters.
        /// </summary>
        /// <param name="shape">Defines area that should be explored.</param>
        /// <param name="collisionType">Defines, which found spatial entities in intersecting shape should be explored.</param>
        /// <returns>All spatial entities with intersecting shape and collision type of the ESC.</returns>
        private List<ISpatialEntity> Explore(IShape shape, Enum collisionType) {
            List<ISpatialEntity> result = new List<ISpatialEntity>();

            foreach (ISpatialEntity foundSpatialObject in _octree.Query(shape.Bounds)) {
                int foundCollisionType = foundSpatialObject.CollisionType.GetHashCode();
                if (Collides(collisionType.GetHashCode(), foundCollisionType)
                    && shape.IntersectsWith(foundSpatialObject.Shape)) {
                    result.Add(foundSpatialObject);
                }
            }
            return result;
        }

        #region IEnvironment Members

        public Vector3 MaxDimension { get; set; }
        public bool IsGrid { get; set; }

        public bool Add(ISpatialEntity entity, Vector3 position, Direction rotation = null) {
            IShape shape = entity.Shape;
            Vector3 movementVector = position - shape.Position;
            MovementResult result = Move(false, entity, movementVector, rotation);
            if (!result.Success) {
                return false;
            }
            return true;
        }

        //TODO auch mit zufälliger Rotation und ohne grid option
        public bool AddWithRandomPosition(ISpatialEntity entity, Vector3 min, Vector3 max, bool grid) {
            for (int attempt = 0; attempt < MaxAttempsToAddRandom; attempt++) {
                Vector3 position = GenerateRandomPosition(min, max, grid);
                bool result = Add(entity, position);
                if (result) {
                    return true;
                }
            }
            return false;
        }

        public void Remove(ISpatialEntity entity) {
            _octree.Remove(entity);
        }

        public bool Resize(ISpatialEntity entity, IShape shape) {
            List<ISpatialEntity> result = Explore(shape, entity.CollisionType);
            result.Remove(entity);
            if (result.Any()) {
                return false;
            }
            _octree.Remove(entity);
            entity.Shape = shape;
            _octree.Insert(entity);
            return true;
        }

        public MovementResult Move(ISpatialEntity entity, Vector3 movementVector, Direction rotation = null) {
            return Move(true, entity, movementVector, rotation);
        }

        public IEnumerable<ISpatialEntity> Explore(ISpatialEntity spatial) {
            return Explore(spatial.Shape, spatial.CollisionType);
        }

        public IEnumerable<ISpatialEntity> ExploreAll() {
            return _octree.GetAll();
        }

        #endregion
    }

}