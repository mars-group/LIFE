using System;
using System.Collections.Generic;
using System.Linq;
using EnvironmentServiceComponent.Entities;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Movement;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Environment;
using SpatialAPI.Shape;
using SpatialObjectOctree.Interface;
using SpatialObjectOctree.Implementation;

namespace EnvironmentServiceComponent.Implementation {

    public class EnvironmentServiceComponent : IEnvironment {
        private const int MaxAttempsToAddRandom = 100;
        private readonly bool[,] _collisionMatrix;
        private readonly IOctree<ISpatialEntity> _octree;
        private readonly Random _random;
        private readonly object _syncLock = new object();

        public EnvironmentServiceComponent(bool[,] collisionMatrix = null) {
            _random = new Random();
            _collisionMatrix = collisionMatrix ?? CollisionMatrix.Get();
            _octree = new SpatialObjectOctree<ISpatialEntity>(new Vector3(25, 25, 25), 1, true);
        }

        #region IEnvironment Members

        public Vector3 MaxDimension { get; set; }
        public bool IsGrid { get; set; }

        public bool Add(ISpatialEntity entity, Vector3 position, Direction rotation = null) {
            IShape shape = entity.Shape;

            rotation = rotation ?? new Direction();
            Vector3 newPosition = position - shape.Position;

            MovementResult result = Move(true, entity, newPosition, rotation);
            if (!result.Success) {
                return false;
            }
            return true;
        }

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
            List<ISpatialEntity> result = Explore(shape, entity.CollisionType).ToList();
            result.Remove(entity);
            if (result.Any()) {
                return false;
            }
            _octree.Remove(entity);
            entity.Shape = shape;
            _octree.Insert(entity);
            return true;
        }

        public MovementResult Move
            (ISpatialEntity entity, Vector3 movementVector, Direction rotation = null) {
            return Move(false, entity, movementVector, rotation);
        }

        public IEnumerable<ISpatialEntity> Explore(ISpatialEntity spatial) {
            return Explore(spatial.Shape, spatial.CollisionType);
        }

        public IEnumerable<ISpatialEntity> ExploreAll() {
            return _octree.GetAll();
        }

        #endregion

        private MovementResult Move
            (bool calledByAdd, ISpatialEntity entity, Vector3 movementVector, Direction rotation = null) {
            rotation = rotation ?? new Direction();
            IShape newShape = entity.Shape.Transform(movementVector, rotation);
            lock (_syncLock) {
                List<ISpatialEntity> collisions = Explore(newShape, entity.CollisionType).ToList();
                collisions.Remove(entity);

                if (collisions.Any()) {
                    return new MovementResult(collisions);
                }
                if (!calledByAdd) {
                    _octree.Remove(entity);
                }
                entity.Shape = newShape;
                _octree.Insert(entity);
                return new MovementResult();
            }
        }

        private bool Collides(int givenCollisionType, int foundCollisionType) {
            return _collisionMatrix[givenCollisionType, foundCollisionType];
        }

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

        private double GetRandomNumber(double min, double max) {
            return _random.NextDouble()*(max - min) + min;
        }

        private IEnumerable<ISpatialEntity> Explore(IShape shape, Enum collisionType) {
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
    }

}