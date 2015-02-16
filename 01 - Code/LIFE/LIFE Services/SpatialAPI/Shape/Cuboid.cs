using System;
using SpatialAPI.Common;
using SpatialAPI.Entities.Transformation;

namespace SpatialAPI.Shape {

    public class Cuboid : IShape {
        private readonly Vector3 _dimension;
        private readonly Vector3 _position;
        private readonly Direction _rotation;
        private readonly BoundingBox _bounds;

        public Cuboid(Vector3 dimension, Vector3 position, Direction rotation = null) {
            _dimension = dimension;
            _position = position;
            _rotation = rotation ?? new Direction();

            AABB aabb = AABB.Generate(Position, Rotation.GetDirectionalVector(), Dimension);
            Vector3 leftBottomFront = new Vector3(aabb.XIntv._min, aabb.YIntv._min, aabb.ZIntv._min);
            Vector3 rightTopRear = new Vector3(aabb.XIntv._max, aabb.YIntv._max, aabb.ZIntv._max);
            _bounds = BoundingBox.GenerateByCorners(leftBottomFront, rightTopRear);

        }

        public Vector3 Dimension { get { return _dimension; } }

        #region IShape Members

        public Direction Rotation { get { return _rotation; } }
        public Vector3 Position { get { return _position; } }

        public BoundingBox Bounds {
            get { return _bounds; }
        }


        public bool IntersectsWith(IShape shape) {
            return shape.Bounds.IntersectsWith((IShape) Bounds);
        }

        public IShape Transform(Vector3 movement, Direction rotation) {
            return new Cuboid(_dimension, Position + movement, rotation);
        }

        public override string ToString() {
            return String.Format("Cuboid({0})", Bounds.ToString());
        }

        #endregion
    }

}