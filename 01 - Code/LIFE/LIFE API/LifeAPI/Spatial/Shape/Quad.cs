using System;
using System.Threading.Tasks;

namespace LifeAPI.Spatial.Shape {

    public class Quad : IShape {
        private readonly TVector _dimension;
        private readonly TVector _position;
        private readonly Direction _rotation;

        public Quad(TVector dimension, TVector position, Direction rotation) {
            _dimension = dimension;
            _position = position;
            _rotation = rotation;
        }

        #region IShape Members

        public TVector GetPosition() {
            return _position;
        }

        public Direction GetRotation() {
            return _rotation;
        }

        public Quad GetBounds() {
            return this;
        }

        public bool CollidesWith(IShape shape) {
            Quad quad = shape as Quad;
            if (quad == null) {
                return CollidesWith(shape.GetBounds());
            }
            AABB me = GetAABB(_position, _rotation.GetDirectionalVector().GetTVector(), _dimension);
            AABB other = GetAABB(quad._position, quad._rotation.GetDirectionalVector().GetTVector(), quad._dimension);
            return me.collidesWith(other);
        }

        public IShape Transform(TVector movement, Direction rotation) {
            return new Quad(_dimension, _position + movement, rotation);
        }

        #endregion

        private static AABB GetAABB(TVector position, TVector direction, TVector dimension) {
            // Create all vertices of the bounding box. Probably some of them will suffice ...
            TVector[] points = new TVector[8];
            points[0] = new TVector(-dimension.X/2, -dimension.Y/2, -dimension.Z/2);
            points[1] = new TVector(dimension.X/2, -dimension.Y/2, -dimension.Z/2);
            points[2] = new TVector(-dimension.X/2, dimension.Y/2, -dimension.Z/2);
            points[3] = new TVector(dimension.X/2, dimension.Y/2, -dimension.Z/2);
            points[4] = new TVector(-dimension.X/2, -dimension.Y/2, dimension.Z/2);
            points[5] = new TVector(dimension.X/2, -dimension.Y/2, dimension.Z/2);
            points[6] = new TVector(-dimension.X/2, dimension.Y/2, dimension.Z/2);
            points[7] = new TVector(dimension.X/2, dimension.Y/2, dimension.Z/2);

            // Build axes for direction-local coordinate system.
            TVector nr1 = direction.Normalize(), nr2, nr3;
            nr1.GetPlanarOrthogonalVectors(out nr2, out nr3);

            // Transform the bounding box from local (direction-aligned) to the
            // absolute coordinate system and get the maximum extent for each axis.
            double diffX = 0, diffY = 0, diffZ = 0;

            Parallel.ForEach
                (points,
                    point => {
                        double x = point.X*nr1.X + point.Y*nr1.Y + point.Z*nr1.Z;
                        double y = point.X*nr2.X + point.Y*nr2.Y + point.Z*nr2.Z;
                        double z = point.X*nr3.X + point.Y*nr3.Y + point.Z*nr3.Z;
                        TVector temp = new TVector(x, y, z);
                        if (temp.X > diffX) {
                            diffX = temp.X;
                        }
                        if (temp.Y > diffY) {
                            diffY = temp.Y;
                        }
                        if (temp.Z > diffZ) {
                            diffZ = temp.Z;
                        }
                    });

            // Create axis-aligned bounding box (AABB) and assign values.
            return new AABB {
                XIntv = new AxisAlignedBoundingInterval(position.X - diffX, position.X + diffX),
                YIntv = new AxisAlignedBoundingInterval(position.Y - diffY, position.Y + diffX),
                ZIntv = new AxisAlignedBoundingInterval(position.Z - diffZ, position.Z + diffZ)
            };
        }

        #region Nested type: AABB

        private struct AABB {
            public AxisAlignedBoundingInterval XIntv, YIntv, ZIntv;

            public override string ToString() {
                return "X-Inv: " + XIntv + "\nY-Inv: " + YIntv + "\nZ-Inv: " + ZIntv;
            }

            public bool collidesWith(AABB aabb) {
                return XIntv.Collide(aabb.XIntv) && YIntv.Collide(aabb.YIntv) && ZIntv.Collide(aabb.ZIntv);
            }
        }

        #endregion

        #region Nested type: AxisAlignedBoundingInterval

        public class AxisAlignedBoundingInterval {
            private readonly double _min;
            private readonly double _max;

            public AxisAlignedBoundingInterval(double val1, double val2) {
                // Set smaller value as minimum.
                if (val1 < val2) {
                    _min = val1;
                    _max = val2;
                }
                else {
                    _min = val2;
                    _max = val1;
                }
            }


            /// <summary>
            ///     Checks for a collision of this and another axis interval.
            ///     In total, there are 12 possible cases of interval alignment.
            ///     Eight of them shall be classified as "overlap".
            /// </summary>
            /// <param name="other">The other interval to check.</param>
            /// <returns>"True": collision, "false": no collision.</returns>
            public bool Collide(AxisAlignedBoundingInterval other) {
                if (ReferenceEquals(this, other)) {
                    return false; //| Overlap with ...  
                }
                if ((_min >= other._min && _min < other._max) || //| 1) left interval
                    (other._min >= _min && other._min < _max) || //| 2) right interval
                    (_min <= other._min && _max >= other._max) || //| 3) inner interval
                    (other._min <= _min && other._max >= _max)) //| 4) outer interval.
                {
                    return true;
                }
                return false;
            }


            public bool Equals(AxisAlignedBoundingInterval other) {
                if (ReferenceEquals(null, other)) {
                    return false;
                }
                if (ReferenceEquals(this, other)) {
                    return true;
                }
                return Math.Abs(_min - other._min) <= float.Epsilon
                       && Math.Abs(_max - other._max) <= float.Epsilon;
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) {
                    return false;
                }
                if (ReferenceEquals(this, obj)) {
                    return true;
                }
                if (obj.GetType() != GetType()) {
                    return false;
                }
                return Equals((AxisAlignedBoundingInterval) obj);
            }

            public override int GetHashCode() {
                unchecked {
                    return (int) (_min*397) ^ (int) _max;
                }
            }

            public static bool operator ==(AxisAlignedBoundingInterval left, AxisAlignedBoundingInterval right) {
                return Equals(left, right);
            }

            public static bool operator !=(AxisAlignedBoundingInterval left, AxisAlignedBoundingInterval right) {
                return !Equals(left, right);
            }

            public override string ToString() {
                return String.Format("({0,6:0.00} →{1,6:0.00})", _min, _max);
            }
        }

        #endregion
    }

}