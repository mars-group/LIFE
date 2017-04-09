using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LIFE.Components.ESC.SpatialAPI.Common;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Movement;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Shape;
using LIFE.Components.ESC.SpatialObjectTree;

namespace LIFE.Components.ESC.Implementation
{
    /// <summary>
    ///   3-dimensional implementation of the IESC interface.
    /// </summary>
    public class TreeESC : IESC, IAsyncEnvironment
    {
        private const int MaxAttempsToAddRandom = 100000;
        private readonly BoundarySpecification _boundarySpecification;
        private readonly bool[,] _collisionMatrix;
        private readonly Random _random = new Random();
        private readonly IDictionary<Guid, ISpatialEntity> _spatialEntities;
        private readonly object _syncLock = new object();
        private readonly ITree<ISpatialEntity> _tree;


        /// <summary>
        ///   Initializes the environemt.
        /// </summary>
        /// <param name="collisionMatrix">Defines the collision behaviour of added ISpatialEntities.</param>
        /// <param name="tree">Is used to store the data</param>
        /// <param name="boundarySpecification">Defines the size and behaviour of boundaries.</param>
        public TreeESC
        (ITree<ISpatialEntity> tree = null,
            BoundarySpecification boundarySpecification = null,
            bool[,] collisionMatrix = null)
        {
            _boundarySpecification = boundarySpecification ?? new BoundarySpecification();
            MaxDimension = _boundarySpecification.Boundary;
            _collisionMatrix = collisionMatrix ?? CollisionMatrix.Get(); // if null use standard behaviour instead
            _tree = tree ?? new SpatialObjectOctree<ISpatialEntity>(new Vector3(25, 25, 25), 1);
            _spatialEntities = new ConcurrentDictionary<Guid, ISpatialEntity>();
        }

        public IEnumerable<ISpatialEntity> Explore(IShape shape, Enum collisionType)
        {
            var result = new List<ISpatialEntity>();
            if (shape != null)
                foreach (var foundSpatialObject in _tree.Query(shape.Bounds))
                {
                    var foundCollisionType = foundSpatialObject.CollisionType.GetHashCode();
                    if (((collisionType == null) || Collides(collisionType.GetHashCode(), foundCollisionType))
                        && shape.IntersectsWith(foundSpatialObject.Shape)) result.Add(foundSpatialObject);
                }
            return result;
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
            rotation = rotation ?? new Direction();
            var newShape = entity.Shape.Transform(movementVector, rotation);
            lock (_syncLock)
            {
                if (EnvBoundaryType.BlockOnExit.Equals(_boundarySpecification.EnvBoundaryType)
                    && !(MaxDimension >= newShape.Position))
                    return new MovementResult(_boundarySpecification.BoundaryEntityInList);

                var collisions = Explore(newShape, entity.CollisionType).ToList();
                collisions.Remove(entity);

                if (collisions.Any()) return new MovementResult(collisions);
                if (entityIsAdded) _tree.Remove(entity);
                entity.Shape = newShape;
                _tree.Insert(entity);

                return new MovementResult();
            }
        }

        /// <summary>
        ///   Indicates if the combination of collision types causes a collision. Respect the order.
        /// </summary>
        /// <param name="givenCollisionType">The collision type that may collide with the other.</param>
        /// <param name="foundCollisionType">The collision type of which the correspondency is checked.</param>
        /// <returns>True, if the combination results in a collision, false otherwise.</returns>
        private bool Collides(int givenCollisionType, int foundCollisionType)
        {
            return _collisionMatrix[givenCollisionType, foundCollisionType];
        }

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

        public override string ToString()
        {
            return string.Format("{0}<{1}>", GetType(), _tree.GetType());
        }

        #region IESC Members

        public Vector3 MaxDimension { get; set; }
        public bool IsGrid { get; set; }

        public bool Add(ISpatialEntity entity, Vector3 position, Direction rotation = null)
        {
            return AddEntity(entity, position, rotation).Success;
        }

        private MovementResult AddEntity(ISpatialEntity entity, Vector3 position, Direction rotation = null)
        {
            var shape = entity.Shape;
            var movementVector = position - shape.Position;
            var result = Move(false, entity, movementVector, rotation);
            if (result.Success) _spatialEntities.Add(entity.AgentGuid, entity);
            return result;
        }

        public bool AddWithRandomPosition(ISpatialEntity entity, Vector3 min, Vector3 max, bool grid)
        {
            return AddEntityWithRandomPosition(entity, min, max, grid).Success;
        }

        public MovementResult AddEntityWithRandomPosition(ISpatialEntity entity, Vector3 min, Vector3 max, bool grid)
        {
            for (var attempt = 0; attempt < MaxAttempsToAddRandom; attempt++)
            {
                var position = GenerateRandomPosition(min, max, grid);
                var result = AddEntity(entity, position);
                if (result.Success)
                {
                    _spatialEntities[entity.AgentGuid] = entity;
                    return result;
                }
            }
            return new MovementResult(Explore(entity));
        }

        public void Add(ISpatialEntity entity, Vector3 position, Direction rotation, MovementDelegate movementDelegate)
        {
            var result = AddEntity(entity, position, rotation);
            movementDelegate(result);
        }

        public void AddWithRandomPosition
            (ISpatialEntity entity, Vector3 min, Vector3 max, bool grid, MovementDelegate movementDelegate)
        {
            var result = AddEntityWithRandomPosition(entity, min, max, grid);
            movementDelegate(result);
        }

        public void Remove(ISpatialEntity entity)
        {
            _tree.Remove(entity);
            _spatialEntities.Remove(entity.AgentGuid);
        }

        public void Resize(ISpatialEntity entity, IShape shape, MovementDelegate movementDelegate)
        {
            var result = ResizeEntity(entity, shape);
            movementDelegate(result);
        }

        public void Move
            (ISpatialEntity entity, Vector3 movementVector, Direction rotation, MovementDelegate movementDelegate)
        {
            Move(entity, movementVector, rotation);
            movementDelegate(new MovementResult());
        }

        public void Explore(ISpatialObject spatial, ExploreDelegate exploreDelegate, Type agentType = null)
        {
            var result = Explore(spatial, agentType);
            exploreDelegate(result);
        }

        public void Explore(IShape shape, ExploreDelegate exploreDelegate, Enum collisionType = null)
        {
            var result = Explore(shape, collisionType);
            exploreDelegate(result);
        }

        public bool Resize(ISpatialEntity entity, IShape shape)
        {
            return ResizeEntity(entity, shape).Success;
        }

        private MovementResult ResizeEntity(ISpatialEntity entity, IShape shape)
        {
            var resizedShape = shape;
            var previousPosition = entity.Shape.Position;
            if (!shape.Position.Equals(previousPosition))
                resizedShape = shape.Transform(previousPosition - shape.Position, shape.Rotation); //TODO
            var resultList = new List<ISpatialEntity>(Explore(resizedShape, entity.CollisionType));
            resultList.Remove(entity);
            var result = new MovementResult(resultList);
            if (!resultList.Any())
            {
                _tree.Remove(entity);
                entity.Shape = resizedShape;
                _tree.Insert(entity);
            }
            return result;
        }

        public MovementResult Move(ISpatialEntity entity, Vector3 movementVector, Direction rotation = null)
        {
            return Move(true, entity, movementVector, rotation);
        }

        public IEnumerable<ISpatialEntity> Explore(ISpatialObject spatial, Type agentType = null)
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

        public void Commit()
        {
            //do nothing
        }

        public IEnumerable<ISpatialEntity> Explore(Sphere spatial, int maxResults = -1)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}