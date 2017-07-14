using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LIFE.Components.ESC.BVH;
using LIFE.Components.ESC.SpatialAPI.Common;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Movement;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace LIFE.Components.ESC.Implementation
{
    /// <summary>
    ///   3-dimensional implementation of the IESC interface.
    /// </summary>
    public class DistributedESC : IESC
    {
        private const int MaxAttempsToAddRandom = 100000;

        private readonly bool[,] _collisionMatrix;
        private readonly Random _random = new Random();

        private readonly ConcurrentDictionary<Guid, ISpatialEntity> _spatialEntities;
        private readonly ITree<ISpatialEntity> _tree;

        /// <summary>
        ///   Initializes the environment.
        /// </summary>
        /// <param name="collisionMatrix">Defines the collision behaviour of added ISpatialEntities.</param>
        /// <param name="maxLeafObjectCount"></param>
        public DistributedESC(bool[,] collisionMatrix = null, int maxLeafObjectCount = 10)
        {
            _collisionMatrix = collisionMatrix ?? CollisionMatrix.Get(); // if null use standard behaviour instead
            _tree = new BoundingVolumeHierarchy<ISpatialEntity>(leafObjectMax: maxLeafObjectCount);
            _spatialEntities = new ConcurrentDictionary<Guid, ISpatialEntity>();
        }

        public IEnumerable<ISpatialEntity> Explore(Sphere sphere, int maxResults = -1)
        {
            return sphere == null ? new List<ISpatialEntity>() : _tree.Query(sphere, maxResults);
        }

        public IEnumerable<ISpatialEntity> Explore(IShape shape, Enum collisionType)
        {
            return shape == null ? new List<ISpatialEntity>() : _tree.Query(shape.Bounds);
            /*var result = new List<ISpatialEntity>();
      
                  if (shape == null) {
                      return result;
                  }
      
                  foreach (var foundSpatialObject in _tree.Query(shape.Bounds)) {
                      var foundCollisionType = foundSpatialObject.CollisionType.GetHashCode();
                      // object does not collide or collisionTypes are setup to not collide, so continue
                      if (collisionType != null && !Collides(collisionType.GetHashCode(), foundCollisionType)) {
                          continue;
                      }
                      // objects CAN collide, so check if they do
                      if (shape.IntersectsWith(foundSpatialObject.Shape)) {
                          result.Add(foundSpatialObject);
                      }
                  }
                  return result;
                  */
        }


        /// <summary>
        ///   Tries to move given entity relatively from it's current position.
        /// </summary>
        /// <param name="entityIsAdded">Indicates if the entity is already in the system.</param>
        /// <param name="entity">That will be moved.</param>
        /// <param name="movementVector">The vector defines a relative transition towards the goal.</param>
        /// <param name="rotation">
        ///   The rotation defines the absolute orientation of the entity. If null, the previous rotation is used but
        ///   not modified.
        /// </param>
        /// <returns>A MovementResult that defines the success of the operation and possible collisions.</returns>
        private MovementResult Move
            (bool entityIsAdded, ISpatialEntity entity, Vector3 movementVector, Direction rotation = null)
        {
            //At move propagation must be done before entity is modified => otherwise replicated octrees won't
            //find the entity since the octree implementation has a lookup table over shapes :-/ and not agent id
            rotation = rotation ?? new Direction();

            var newShape = entity.Shape.Transform(movementVector, rotation);

            // will only remove if entity is in the octree
            if (entityIsAdded)
                _tree.Remove(entity);


            entity.Shape = newShape;
            _tree.Insert(entity);

            return new MovementResult();
        }

/*
    /// <summary>
    ///   Indicates if the combination of collision types causes a collision. Respect the order.
    /// </summary>
    /// <param name="givenCollisionType">The collision type that may collide with the other.</param>
    /// <param name="foundCollisionType">The collision type of which the correspondency is checked.</param>
    /// <returns>True, if the combination results in a collision, false otherwise.</returns>
    private bool Collides(int givenCollisionType, int foundCollisionType) {
      return _collisionMatrix[givenCollisionType, foundCollisionType];
    }
*/

        /// <summary>
        ///   Generates a random position within the given bounds.
        /// </summary>
        /// <param name="min">Lower boundary.</param>
        /// <param name="max">Upper boundary.</param>
        /// <param name="grid">Defines, if position only holds integer values.</param>
        /// <returns>The generated position.</returns>
        private Vector3 GenerateRandomPosition(Vector3 min, Vector3 max, bool grid)
        {
            if (grid)
            {
                var x = _random.Next((int) min.X, (int) max.X + 1);
                var y = _random.Next((int) min.Y, (int) max.Y + 1);
                var z = _random.Next((int) min.Z, (int) max.Z + 1);
                return new Vector3(x, y, z);
            }
            else
            {
                var x = GetRandomNumber(min.X, max.X);
                var y = GetRandomNumber(min.Y, max.Y);
                var z = GetRandomNumber(min.Z, max.Z);
                return new Vector3(x, y, z);
            }
        }

        /// <summary>
        ///   Provides a random number within the given range.
        /// </summary>
        /// <param name="min">Lower boundary.</param>
        /// <param name="max">Upper boundary.</param>
        /// <returns>The generated random number.</returns>
        private double GetRandomNumber(double min, double max)
        {
            return _random.NextDouble() * (max - min) + min;
        }

        #region IESC Members

        public Vector3 MaxDimension { get; set; }
        public bool IsGrid { get; set; }

        public bool Add(ISpatialEntity entity, Vector3 position, Direction rotation = null, bool propagate = true)
        {
            var shape = entity.Shape;
            var movementVector = position - shape.Position;

            // call to internal method directly and DONT propagate
            var result = Move(false, entity, movementVector, rotation);

            if (!result.Success) return false;

            _spatialEntities.AddOrUpdate(entity.AgentGuid, entity, (key, old) => entity);

            return true;
        }

        //TODO auch mit zufälliger Rotation und ohne grid option
        public bool AddWithRandomPosition
        (ISpatialEntity entity,
            Vector3 min,
            Vector3 max,
            bool grid,
            bool propagate = true)
        {
            for (var attempt = 0; attempt < MaxAttempsToAddRandom; attempt++)
            {
                var position = GenerateRandomPosition(min, max, grid);
                var result = Add(entity, position, null, propagate = true);
                if (result)
                {
                    _spatialEntities[entity.AgentGuid] = entity;
                    return true;
                }
            }

            return false;
        }

        public void Remove(ISpatialEntity entity, bool propagate = true)
        {
            _tree.Remove(entity);
            _spatialEntities.TryRemove(entity.AgentGuid, out entity);
        }

        public bool Resize(ISpatialEntity entity, IShape shape, bool propagate = true)
        {
            //At resize propagation must be done before entity is modified => otherwise replicated octrees won't
            //find the entity since the octree implementation has a lookup table over shapes :-/ and not agent id


            var result = new List<ISpatialEntity>(Explore(shape, entity.CollisionType));
            result.Remove(entity);
            if (result.Any()) return false;
            _tree.Remove(entity);
            entity.Shape = shape;
            _tree.Insert(entity);

            return true;
        }

        public IEnumerable<ISpatialEntity> Explore(ISpatialEntity spatial, Type agentType = null)
        {
            if ((spatial == null) || (spatial.Shape == null)) return new List<ISpatialEntity>();
            if (agentType == null) return Explore(spatial.Shape, spatial.CollisionType);
            return Explore(spatial.Shape, spatial.CollisionType)
                .Where(entity => entity.AgentType == agentType)
                .ToList();
        }

        public IEnumerable<ISpatialEntity> ExploreAll()
        {
            return _tree.GetAll();
        }


        public bool Add(ISpatialEntity entity, Vector3 position, Direction rotation = null)
        {
            return Add(entity, position, rotation, true);
        }

        public bool AddWithRandomPosition(ISpatialEntity entity, Vector3 min, Vector3 max, bool grid)
        {
            return AddWithRandomPosition(entity, min, max, grid, true);
        }

        public void Remove(ISpatialEntity entity)
        {
            Remove(entity, true);
        }

        public bool Resize(ISpatialEntity entity, IShape shape)
        {
            return Resize(entity, shape, true);
        }

        public MovementResult Move(ISpatialEntity entity, Vector3 movementVector, Direction rotation = null)
        {
            return Move(true, entity, movementVector, rotation);
        }

        #endregion
    }
}