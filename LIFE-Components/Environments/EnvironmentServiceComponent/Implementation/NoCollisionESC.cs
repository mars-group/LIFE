using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Movement;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Shape;
using LIFE.Components.ESC.SpatialObjectTree;

namespace LIFE.Components.ESC.Implementation
{
    /// <summary>
    ///   An <code>IESC</code> that has no collisions on placing or movement of entities. Explore always matches
    ///   entities in the explored area regardless the collision type, but is only exact for shapes that are
    ///   <code>BoundingBox</code>es, without rotation.
    /// </summary>
    public class NoCollisionESC : IESC
    {
        private readonly Random _random = new Random();
        private readonly IDictionary<Guid, ISpatialEntity> _spatialEntities;
        private readonly ITree<ISpatialEntity> _tree;

        /// <summary>
        ///   Initializes the environemt.
        /// </summary>
        /// <param name="tree">that is used to store the <code>ISpatialEntity</code>s</param>
        public NoCollisionESC(ITree<ISpatialEntity> tree = null)
        {
            _tree = tree ?? new SpatialObjectOctree<ISpatialEntity>(new Vector3(25, 25, 25), 1);
            _spatialEntities = new ConcurrentDictionary<Guid, ISpatialEntity>();
        }

        public NoCollisionESC()
        {
            _tree = new SpatialObjectOctree<ISpatialEntity>(new Vector3(25, 25, 25), 1);
            _spatialEntities = new ConcurrentDictionary<Guid, ISpatialEntity>();
        }

        public IEnumerable<ISpatialEntity> Explore(IShape shape, Enum collisionType)
        {
            return shape == null ? new List<ISpatialEntity>() : _tree.Query(shape.Bounds);
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

            if (entityIsAdded) _tree.Remove(entity);
            entity.Shape = newShape;
            _tree.Insert(entity);

            return new MovementResult();
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
            return GetType() + "<" + _tree.GetType() + ">";
        }

        #region IESC Members

        public Vector3 MaxDimension { get; set; }
        public bool IsGrid { get; set; }

        public bool Add(ISpatialEntity entity, Vector3 position, Direction rotation = null)
        {
            var shape = entity.Shape;
            var movementVector = position - shape.Position;
            Move(false, entity, movementVector, rotation);
            _spatialEntities.Add(entity.AgentGuid, entity);
            return true;
        }

        public bool AddWithRandomPosition(ISpatialEntity entity, Vector3 min, Vector3 max, bool grid)
        {
            var position = GenerateRandomPosition(min, max, grid);
            Add(entity, position);
            _spatialEntities[entity.AgentGuid] = entity;
            return true;
        }

        public void Remove(ISpatialEntity entity)
        {
            _tree.Remove(entity);
            _spatialEntities.Remove(entity.AgentGuid);
        }

        //        public void Add(ISpatialEntity entity, Vector3 position, Direction rotation, MovementDelegate movementDelegate)
        //        {
        //            Add(entity, position, rotation);
        //            movementDelegate(new MovementResult());
        //        }
        //
        //        public void AddWithRandomPosition
        //            (ISpatialEntity entity, Vector3 min, Vector3 max, bool grid, MovementDelegate movementDelegate)
        //        {
        //            AddWithRandomPosition(entity, min, max, grid);
        //            movementDelegate(new MovementResult());
        //        }
        //
        //        public void Remove(ISpatialEntity entity)
        //        {
        //            _tree.Remove(entity);
        //            _spatialEntities.Remove(entity.AgentGuid);
        //        }
        //
        //        public void Resize(ISpatialEntity entity, IShape shape, MovementDelegate movementDelegate)
        //        {
        //            Resize(entity, shape);
        //            movementDelegate(new MovementResult());
        //        }
        //
        //        public void Move
        //            (ISpatialEntity entity, Vector3 movementVector, Direction rotation, MovementDelegate movementDelegate)
        //        {
        //            Move(entity, movementVector, rotation);
        //            movementDelegate(new MovementResult());
        //        }
        //
        //        public void Explore(ISpatialObject spatial, ExploreDelegate exploreDelegate, Type agentType = null)
        //        {
        //            var result = Explore(spatial, agentType);
        //            exploreDelegate(result);
        //        }
        //
        //        public void Explore(IShape shape, ExploreDelegate exploreDelegate, Enum collisionType = null)
        //        {
        //            var result = Explore(shape, collisionType);
        //            exploreDelegate(result);
        //        }

        public bool Resize(ISpatialEntity entity, IShape shape)
        {
            _tree.Remove(entity);
            entity.Shape = shape;
            _tree.Insert(entity);
            return true;
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