using System;
using System.Collections.Generic;
using System.Linq;
using EnvironmentServiceComponent.Entities;
using LifeAPI.Environment;
using LifeAPI.Spatial;
using SpatialCommon.Shape;
using SpatialCommon.Transformation;

namespace EnvironmentServiceComponent.Implementation {

    public class EnviromentServiceComponent :IEnvironment{
        private const int MaxAttempsToAddRandom = 100;
        private readonly Random _random;
        private readonly bool[,] _collisionMatrix;
        private readonly Dictionary<IShape, ISpatialEntity> _entities;
//        private readonly OctreeFlo<IShape> _quadTree;


        public EnviromentServiceComponent(bool[,] collisionMatrix = null) {
            _random = new Random();
            _collisionMatrix = collisionMatrix ?? CollisionMatrix.Get();

            _entities = new Dictionary<IShape, ISpatialEntity>();
//            _quadTree = new OctreeFlo<IShape>(new Vector3(25, 25, 25), 1, true);
        }


        public Vector3 MaxDimension { get; set; }
        public bool IsGrid { get; set; }

        public bool Add(ISpatialEntity entity, Vector3 position, Direction rotation = null) {
            IShape shape = entity.Shape;
            if (rotation != null) {
                Vector3 initRotation = rotation.GetDirectionalVector() - shape.Rotation.GetDirectionalVector();
                rotation.SetDirectionalVector(initRotation);
            }
            Vector3 newPosition = position - shape.Position;

            //move to position
            MovementResult result = Move(true, entity, newPosition, rotation);
            if (!result.Success) {
                return false;
            }
            IShape newShape = entity.Shape.Transform(newPosition, rotation);
            entity.Shape = newShape;
            _entities.Add(newShape, entity);
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
            _entities.Remove(entity.Shape);
//                _quadTree.Remove(shape);
        }

        public bool Resize(ISpatialEntity entity, IShape shape) {
            ExploreEntity exploreEntity = new ExploreEntity(shape, entity.CollisionType);
            List<ISpatialEntity> result = Explore(exploreEntity).ToList();
            result.Remove(entity);
            if (result.Any()) {
                return false;
            }
//            _quadTree.Remove(oldShape);
//            _quadTree.Insert(newShape);
            entity.Shape = shape;
            return true;
        }

        private MovementResult Move
            (bool calledByAdd, ISpatialEntity entity, Vector3 movementVector, Direction rotation = null) {
            IShape newShape = entity.Shape.Transform(movementVector, rotation);
            ExploreEntity exploreEntity = new ExploreEntity(newShape, entity.CollisionType);

            List<ISpatialEntity> collisions = Explore(exploreEntity).ToList();
            collisions.Remove(entity);
            if (collisions.Any()) {
                return new MovementResult(collisions);
            }
            if (!calledByAdd) {
//                _quadTree.Remove(rectShape);
            }
//            _quadTree.Insert(rectShape);
            entity.Shape = newShape;
            return new MovementResult();
        }

        public MovementResult Move
            (ISpatialEntity entity, Vector3 movementVector, Direction rotation = null) {
            return Move(false, entity, movementVector, rotation);
        }

        public IEnumerable<ISpatialEntity> Explore(ISpatialEntity spatial) {
            List<ISpatialEntity> result = new List<ISpatialEntity>();
//            foreach (IShape rectShape in _quadTree.Query(exploreShape.Bounds)) {
//                int givenCollisionType = spatial.GetCollisionType().GetHashCode();
//                int foundCollisionType = _entities[rectShape].GetCollisionType().GetHashCode();
//                if (Collides(givenCollisionType, foundCollisionType)) {
//                    result.Add(_entities[rectShape]);
//                }
//            }
            return result;
        }

        public IEnumerable<ISpatialEntity> ExploreAll() {
            return _entities.Values.ToList();
        }


        protected bool Collides(int givenCollisionType, int foundCollisionType) {
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
    }

    internal class ExploreEntity : ISpatialEntity {
        private readonly Enum _collisionType;

        public ExploreEntity(IShape shape, Enum collisionType) {
            _collisionType = collisionType;
            Shape = shape;
        }

        #region ISpatialEntity Members

        public Enum InformationType { get { return CollisionType; } }
        public IShape Shape { get; set; }
        public Enum CollisionType { get { return _collisionType; } }

        #endregion
    }

}