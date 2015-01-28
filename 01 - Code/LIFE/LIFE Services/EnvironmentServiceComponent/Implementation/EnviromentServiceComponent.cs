using System;
using System.Collections.Generic;
using System.Linq;
using EnvironmentServiceComponent.Entities;
using LifeAPI.Environment;
using LifeAPI.Spatial;
using OctreeFlo.Implementation;
using OctreeFlo.Interface;
using SpatialCommon.Shape;
using SpatialCommon.Transformation;
using Direction = SpatialCommon.Transformation.Direction;

namespace EnvironmentServiceComponent.Implementation {

    public class EnviromentServiceComponent : IEnvironment {
        private const int MaxAttempsToAddRandom = 100;
        private readonly bool[,] _collisionMatrix;
        private readonly Dictionary<IShape, ISpatialEntity> _entities;
        private readonly IOctreeFlo<IShape> _octree;
        private readonly Random _random;

        public EnviromentServiceComponent(bool[,] collisionMatrix = null) {
            _random = new Random();
            _collisionMatrix = collisionMatrix ?? CollisionMatrix.Get();

            _entities = new Dictionary<IShape, ISpatialEntity>();
            _octree = new OctreeFlo<IShape>(new Vector3(25, 25, 25), 1, true);
        }

        public Vector3 MaxDimension { get; set; }
        public bool IsGrid { get; set; }

        public bool Add(ISpatialEntity entity, Vector3 position, Direction rotation = null) {
            IShape shape = entity.Shape;

            rotation = rotation ?? new Direction();
            Vector3 newPosition = position - shape.Position;

            //move to position
            MovementResult result = Move(true, entity, newPosition, rotation);
            if (!result.Success) {
                return false;
            }
            IShape newShape = entity.Shape.Transform(newPosition, rotation);
//            Console.WriteLine(String.Format("set shapes old {0}    and new {1}",entity.Shape.Bounds,newShape.Bounds));
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
            _octree.Remove(entity.Shape);
        }

        public bool Resize(ISpatialEntity entity, IShape shape) {
            ExploreEntity exploreEntity = new ExploreEntity(shape, entity.CollisionType);
            List<ISpatialEntity> result = Explore(exploreEntity).ToList();
            result.Remove(entity);
            if (result.Any()) {
                return false;
            }
            _octree.Remove(entity.Shape);
            _octree.Insert(shape);
            entity.Shape = shape;
            return true;
        }

        public MovementResult Move
            (ISpatialEntity entity, Vector3 movementVector, Direction rotation = null) {
            return Move(false, entity, movementVector, rotation);
        }

        public IEnumerable<ISpatialEntity> Explore(ISpatialEntity spatial) {
            List<ISpatialEntity> result = new List<ISpatialEntity>();
            var shape = spatial.Shape;

            Console.WriteLine(" Explore "+shape);
            foreach (IShape foundShape in _octree.Query(shape.Bounds)) {
                int givenCollisionType = spatial.CollisionType.GetHashCode();
                int foundCollisionType = _entities[foundShape].CollisionType.GetHashCode();
                if (Collides(givenCollisionType, foundCollisionType) && shape.IntersectsWith(foundShape)) {
                    result.Add(_entities[foundShape]);
                }
                Console.WriteLine(" shape" + shape + " foundShape" + foundShape);
                Console.WriteLine("Collides(givenCollisionType, foundCollisionType)" + Collides(givenCollisionType, foundCollisionType));
                Console.WriteLine("shape.IntersectsWith(foundShape)" + shape.IntersectsWith(foundShape));

            }
            return result;
        }

        public IEnumerable<ISpatialEntity> ExploreAll() {
            return _entities.Values.ToList();
        }

        private MovementResult Move
            (bool calledByAdd, ISpatialEntity entity, Vector3 movementVector, Direction rotation = null) {
            rotation = rotation ?? new Direction();
            Console.WriteLine("Move movementVector " + movementVector);
            Console.WriteLine("Move rotation " + rotation.GetDirectionalVector());
            IShape newShape = entity.Shape.Transform(movementVector, rotation);
            ExploreEntity exploreEntity = new ExploreEntity(newShape, entity.CollisionType);
            Console.WriteLine(String.Format("Move shapes old {0}    and new {1}", entity.Shape.Bounds, newShape.Bounds));

            Console.WriteLine("Move 1");
            List<ISpatialEntity> collisions = Explore(exploreEntity).ToList();
            collisions.Remove(entity);
            Console.WriteLine("Move 2");
            if (collisions.Any()) {
                return new MovementResult(collisions);
            }
            Console.WriteLine("Move 3");
            if (!calledByAdd) {
                _octree.Remove(entity.Shape);
            }
            Console.WriteLine("Move 4 Insert into octree "+newShape);
            _octree.Insert(newShape);
            Console.WriteLine("Move 5");
            entity.Shape = newShape;
            return new MovementResult();
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