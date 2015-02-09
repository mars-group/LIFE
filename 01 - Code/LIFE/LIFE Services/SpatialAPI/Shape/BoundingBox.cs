using System;
using System.Collections;
using System.Collections.Generic;
using SpatialAPI.Common;
using SpatialAPI.Entities.Transformation;

namespace SpatialAPI.Shape {

    /// <summary>
    ///     Two vectors, defining marking the vertices of an (axis aligned) bounding box.
    /// </summary>
    /// <remarks>
    ///     Notice, that the LeftBottomFront boundary values are inclusive, while the RightTopRear are exclusive.
    ///     This is to avoid ambiguity in Oct responsibility.
    /// </remarks>
    public class BoundingBox : IEnumerable<Vector3>, IShape {
        private readonly Vector3[] _vertices;
        private AABB _aabb;
        private Vector3 _leftBottomFront;
        private Vector3 _rightTopRear;

        /// <summary>
        ///     Creates a new bounding box by using it's edges.
        /// </summary>
        /// <param name="leftBottomFront">The edge at left, bottom and front.</param>
        /// <param name="rightTopRear">The edge at right, top and rear.</param>
        /// <returns>The created bounding box.</returns>
        private BoundingBox(Vector3 leftBottomFront, Vector3 rightTopRear) {
            _vertices = new Vector3[8];
            LeftBottomFront = leftBottomFront;
            RightTopRear = rightTopRear;
        }

        public Vector3 LeftBottomFront {
            get { return _leftBottomFront; }
            private set {
                _leftBottomFront = value;
                RecalculateVertices();
            }
        }

        public Vector3 RightTopRear {
            get { return _rightTopRear; }
            private set {
                _rightTopRear = value;
                RecalculateVertices();
            }
        }

        public Vector3 Dimension { get; private set; }
        public double Width { get { return Dimension.X; } }
        public double Height { get { return Dimension.Y; } }
        public double Length { get { return Dimension.Z; } }

        /// <summary>
        ///     Creates a new bounding box by using it's edges.
        /// </summary>
        /// <param name="leftBottomFront">The edge at left, bottom and front.</param>
        /// <param name="rightTopRear">The edge at right, top and rear.</param>
        /// <returns>The created bounding box.</returns>
        public static BoundingBox GenerateByCorners(Vector3 leftBottomFront, Vector3 rightTopRear) {
            return new BoundingBox(leftBottomFront, rightTopRear);
        }

        /// <summary>
        ///     Creates a new bounding box by defining position and dimension.
        /// </summary>
        /// <param name="position">Defines the ceneter point of the bounding box.</param>
        /// <param name="dimension">Defines the dimensions of the bounding box.</param>
        /// <returns>The created bounding box.</returns>
        public static BoundingBox GenerateByDimension(Vector3 position, Vector3 dimension) {
            return new BoundingBox(position - dimension/2, position + dimension/2);
        }

        private void RecalculateVertices() {
            _vertices[0] = LeftBottomFront;
            _vertices[1] = new Vector3(RightTopRear.X, LeftBottomFront.Y, LeftBottomFront.Z);
            _vertices[2] = new Vector3(LeftBottomFront.X, RightTopRear.Y, LeftBottomFront.Z);
            _vertices[3] = new Vector3(RightTopRear.X, RightTopRear.Y, LeftBottomFront.Z);
            _vertices[4] = new Vector3(LeftBottomFront.X, LeftBottomFront.Y, RightTopRear.Z);
            _vertices[5] = new Vector3(RightTopRear.X, LeftBottomFront.Y, RightTopRear.Z);
            _vertices[6] = new Vector3(LeftBottomFront.X, RightTopRear.Y, RightTopRear.Z);
            _vertices[7] = RightTopRear;

            var dx = RightTopRear.X - LeftBottomFront.X;
            var dy = RightTopRear.Y - LeftBottomFront.Y;
            var dz = RightTopRear.Z - LeftBottomFront.Z;
            Dimension = new Vector3(dx, dy, dz);
            Position = LeftBottomFront + new Vector3(dx/2, dy/2, dz/2);
            _aabb = AABB.Generate(Position, Rotation.GetDirectionalVector(), Dimension);
        }

        /// <summary>
        ///     Returns if the other bounding box intersects with this bounding box.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IntersectsWith(BoundingBox other) {
            return _aabb.IntersectsWith(other._aabb);
        }

        /// <summary>
        ///     Returns true, if the line between the given vectors crosses through this box.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool Crosses(Vector3 from, Vector3 to) {
            /* Basically, for each axis a:
             *  if the start and end point's value are within our own boundaries
             *  and thesame is true for any of the other axes, the line crosses us.
             * Works only, because bounding boxes are axis aligned, i.e. no diagonal lines.
             */
            return
                (from.X >= LeftBottomFront.X && to.X < RightTopRear.X &&
                 (from.Y >= LeftBottomFront.Y && to.Y < RightTopRear.Y)
                 ||
                 (from.Z >= LeftBottomFront.Z && to.Z < RightTopRear.Z)
                    )
                ||
                (from.Y >= LeftBottomFront.Y && to.Y < RightTopRear.Y &&
                 (from.X >= LeftBottomFront.X && to.X < RightTopRear.X)
                 ||
                 (from.Z >= LeftBottomFront.Z && to.Z < RightTopRear.Z)
                    )
                ||
                (from.Z >= LeftBottomFront.Z && to.Z < RightTopRear.Z &&
                 (from.X >= LeftBottomFront.X && to.X < RightTopRear.X)
                 ||
                 (from.Y >= LeftBottomFront.Y && to.Y < RightTopRear.Y)
                    );
        }

        public bool Surrounds(Vector3 position) {
            var result = position.X >= LeftBottomFront.X && position.X < RightTopRear.X &&
                         position.Y >= LeftBottomFront.Y && position.Y < RightTopRear.Y &&
                         position.Z >= LeftBottomFront.Z && position.Z < RightTopRear.Z;
            return result;
        }

        public BoundingBox Copy() {
            return GenerateByDimension(Position, Dimension);
        }

        public bool Contains(BoundingBox other) {
            return Contains(this, other);
        }

        public static bool Contains(BoundingBox box1, BoundingBox box2) {
            return box1.LeftBottomFront <= box2.LeftBottomFront && box2.RightTopRear <= box1.RightTopRear;
        }

        public override string ToString() {
            return String.Format("BoundingBox({0}->{1})", LeftBottomFront, RightTopRear);
        }

//        public override bool Equals(object obj) {
//            if (obj == null || GetType() != obj.GetType()) {
//                return false;
//            }
//            var other = (BoundingBox) obj;
//            return LeftBottomFront.Equals(other.LeftBottomFront) && RightTopRear.Equals(other.RightTopRear);
//        }
//
//        public override int GetHashCode() {
//            return LeftBottomFront.GetHashCode() + RightTopRear.GetHashCode();
//        }

        #region IEnumerable<Vector3> Members

        public IEnumerator<Vector3> GetEnumerator() {
            return ((IEnumerable<Vector3>) _vertices).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

        #region IShape Members

        public Vector3 Position { get; private set; }

        public Direction Rotation { get { return new Direction(); } }

        public BoundingBox Bounds { get { return this; } }

        public bool IntersectsWith(IShape shape) {
            var boundingBox = shape as BoundingBox;
            if (boundingBox == null) {
                return IntersectsWith((IShape) shape.Bounds);
            }
            return IntersectsWith(boundingBox);
        }

        public IShape Transform(Vector3 movement, Direction rotation) {
            return new Cuboid(Dimension, Position, Rotation).Transform(movement, rotation).Bounds;
        }

        #endregion
    }

}