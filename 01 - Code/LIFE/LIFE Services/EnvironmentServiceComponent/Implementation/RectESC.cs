using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CSharpQuadTree;
using EnvironmentServiceComponent.Entities.Shape;
using LifeAPI.Spatial;
using LifeAPI.Perception;

namespace EnvironmentServiceComponent.Implementation {

    public class RectESC : ACollisionESC {
        private readonly Dictionary<RectShape, ISpatialEntity> _entities;
        private readonly QuadTree<RectShape> _quadTree;


        public RectESC(bool[,] collisionMatrix = null)
            : base(collisionMatrix) {
            _entities = new Dictionary<RectShape, ISpatialEntity>();
            _quadTree = new QuadTree<RectShape>(new Size(25, 25), 1, true);
        }


        public override bool Add(ISpatialEntity entity, TVector position, TVector rotation = default(TVector)) {
            RectShape shape = entity.Shape as RectShape;
            if (shape != null) {
                //move to position
                MovementResult result = Move(entity, position, rotation, true);
                if (!result.Success) {
                    return false;
                }
                Rect oldBounds = shape.Bounds;
                Rect newBounds = new Rect
                    (position.X - oldBounds.Width/2, position.Y - oldBounds.Height/2, oldBounds.Width, oldBounds.Height);
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

        public override bool Resize(ISpatialEntity entity, IShapeOld shape) {
            RectShape oldShape = entity.Shape as RectShape;
            RectShape newShape = shape as RectShape;
            if (oldShape == null || newShape == null) {
                return false;
            }
            List<ISpatialEntity> result =
                Explore(new ExploreSpatialObject(newShape.Bounds, entity.GetCollisionType())).ToList();
            result.Remove(entity);
            if (result.Any()) {
                return false;
            }
            _quadTree.Remove(oldShape);
            _quadTree.Insert(newShape);
            entity.Shape = shape;
            return true;
        }

        private MovementResult Move(ISpatialEntity entity, TVector movementVector, TVector rotation, bool calledByAdd) {
            RectShape rectShape = entity.Shape as RectShape;
            if (rectShape == null) {
                throw new NotImplementedException();
            }
            Rect oldBounds = rectShape.Bounds;
            TVector oldPosition = rectShape.GetPosition();
            Rect newBounds = MyRectFactory.Rectangle
                (new TVector(oldPosition.X + movementVector.X, oldPosition.Y + movementVector.Y),
                    new TVector(oldBounds.Width, oldBounds.Height));

            ExploreSpatialObject newShape = new ExploreSpatialObject(newBounds, entity.GetCollisionType());
            List<ISpatialEntity> collisions =
                Explore(newShape).ToList();
            collisions.Remove(entity);
            if (collisions.Any()) {
                return new MovementResult(collisions);
            }
            if (!calledByAdd) {
                _quadTree.Remove(rectShape);
            }

            rectShape.Bounds = newBounds;
            _quadTree.Insert(rectShape);
            entity.Shape = rectShape;
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
            List<ISpatialEntity> result = new List<ISpatialEntity>();
            foreach (RectShape rectShape in _quadTree.Query(exploreShape.Bounds)) {
                int givenCollisionType = spatial.GetCollisionType().GetHashCode();
                int foundCollisionType = _entities[rectShape].GetCollisionType().GetHashCode();
                if (Collides(givenCollisionType, foundCollisionType)) {
                    result.Add(_entities[rectShape]);
                }
            }
            return result;
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

            public ExploreSpatialObject(Rect geometry, Enum collisionType) {
                _collisionType = collisionType;
                Shape = new ExploreRectShape(geometry);
            }

            #region ISpatialObject Members

            public IShapeOld Shape { get; set; }

            public Enum GetInformationType() {
                throw new NotImplementedException();
            }

            public Enum GetCollisionType() {
                return _collisionType;
            }

            #endregion
        }

        #endregion
    }

}