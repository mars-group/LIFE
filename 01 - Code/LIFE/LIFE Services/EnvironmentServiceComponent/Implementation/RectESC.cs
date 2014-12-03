using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CSharpQuadTree;
using EnvironmentServiceComponent.Entities;
using EnvironmentServiceComponent.Entities.Shape;
using SpatialCommon.Enums;
using SpatialCommon.Interfaces;
using SpatialCommon.TransportTypes;

namespace EnvironmentServiceComponent.Implementation {

    public class RectESC : ACollisionESC {
        private readonly Dictionary<RectShape, ISpatialEntity> _entities;
        private readonly QuadTree<RectShape> _quadTree;
        private readonly bool[,] _collisionMatrix = new bool[,] { { false, false, false, false }, { false, true, false, false }, { false, false, false, true }, { false, false, true, true } };

        public RectESC()
            : base() {
            _entities = new Dictionary<RectShape, ISpatialEntity>();
            _quadTree = new QuadTree<RectShape>(new Size(25, 25), 1, true);
        }


        public override bool Add(ISpatialEntity entity, TVector position, TVector rotation = default(TVector)) {
            RectShape shape = entity.Shape as RectShape;
            if (shape != null) {
                Rect oldBounds = shape.Bounds;
                Rect newBounds = new Rect
                    (position.X - oldBounds.Width/2, position.Y + oldBounds.Height/2, oldBounds.Width, oldBounds.Height);

                //move to position
                MovementResult result = Move(entity, position, rotation, true);
                if (!result.Success) {
                    return false;
                }
                shape.Bounds = newBounds;
                _entities.Add(shape, entity);
                return true;
            }
            return false;
        }


        public override void Remove(ISpatialEntity entity) {
            RectShape shape = entity.Shape as RectShape;
            if (shape != null) {
                _entities.Remove(shape);
                _quadTree.Remove(shape);
            }
        }

        public override bool Resize(ISpatialEntity entity, IShape shape) {
            RectShape oldShape = entity.Shape as RectShape;
            RectShape newShape = shape as RectShape;
            if (oldShape == null || newShape == null) {
                return false;
            }
            List<ISpatialEntity> result = Explore(new ExploreSpatialObject(newShape.Bounds, entity.GetCollisionType())).ToList();
            result.Remove(entity);
            if (result.Any()) {
                return false;
            }
            _quadTree.Remove(oldShape);
            _quadTree.Insert(newShape);
            entity.Shape = shape;
            return true;
        }

        private MovementResult Move
            (ISpatialEntity entity, TVector movementVector, TVector rotation, bool calledByAdd) {
            RectShape rectShape = entity.Shape as RectShape;
            if (rectShape == null) {
                throw new NotImplementedException();
            }

            Rect oldBounds = rectShape.Bounds;
            TVector oldPosition = entity.Shape.GetPosition();
            Rect newBounds = new Rect
                (oldPosition.X + movementVector.X - oldBounds.Width/2,
                    oldPosition.Y + movementVector.Y + oldBounds.Height/2,
                    oldBounds.Width,
                    oldBounds.Height);


            List<ISpatialEntity> collisions = Explore(new ExploreSpatialObject(newBounds, entity.GetCollisionType())).ToList();
            collisions.Remove(entity);
            if (collisions.Any()) {
                return new MovementResult(collisions);
            }
            if (!calledByAdd) {
                _quadTree.Remove(rectShape);
            }

            rectShape.Bounds = newBounds;
            _quadTree.Insert(rectShape);
            return new MovementResult();
        }

        public override MovementResult Move
            (ISpatialEntity entity, TVector movementVector, TVector rotation = default(TVector)) {
            return Move(entity, movementVector, rotation, false);
        }

        public override IEnumerable<ISpatialEntity> Explore(ISpatialObject spatial) {
            RectShape exploreShape = spatial.Shape as RectShape;
            if (exploreShape == null) {
                throw new NotImplementedException();
            }

            List<ISpatialEntity> entities = new List<ISpatialEntity>();
            foreach (RectShape rectShape in _quadTree.Query(exploreShape.Bounds))
            {
                Console.WriteLine(spatial.GetCollisionType());
                Console.WriteLine(_entities[rectShape].GetCollisionType());
                Console.WriteLine(_collisionMatrix[spatial.GetCollisionType().GetHashCode(), _entities[rectShape].GetCollisionType().GetHashCode()]);
                if (_collisionMatrix[spatial.GetCollisionType().GetHashCode(), _entities[rectShape].GetCollisionType().GetHashCode()])
                {
                    entities.Add(_entities[rectShape]);
                }
            }

            return entities;
        }

        public override IEnumerable<ISpatialEntity> ExploreAll() {
            return _entities.Values.ToList();
        }

        public override object GetData(ISpecification spec) {
            ISpatialObject spatialObject = spec as ISpatialObject;
            if (spatialObject != null) {
                return Explore(spatialObject);
            }
            return null;
        }

        #region Nested type: ExploreSpatialObject

        private class ExploreSpatialObject : ISpatialObject {
            private readonly Enum _collisionType;

            public ExploreSpatialObject(Rect geometry, Enum collisionType)
            {
                _collisionType = collisionType;
                Shape = new ExploreRectShape(geometry);
            }

            #region ISpatialObject Members

            public IShape Shape { get; set; }

            public Enum GetInformationType() {
                throw new NotImplementedException();
            }

            #endregion

            public Enum GetCollisionType() {
                return _collisionType;
            }
        }

        #endregion
    }

}