using System;
using SpatialAPI.Entities.Transformation;

namespace SpatialAPI.Common {

    internal struct AABB {
        public AxisAlignedBoundingInterval XIntv, YIntv, ZIntv;

        public static AABB Generate(Vector3 position, Vector3 direction, Vector3 dimension) {
            // Create all vertices of the bounding box. Probably some of them will suffice ...
            Vector3[] points = new Vector3[8];
            points[0] = new Vector3(-dimension.X/2, -dimension.Y/2, -dimension.Z/2);
            points[1] = new Vector3(dimension.X/2, -dimension.Y/2, -dimension.Z/2);
            points[2] = new Vector3(-dimension.X/2, dimension.Y/2, -dimension.Z/2);
            points[3] = new Vector3(dimension.X/2, dimension.Y/2, -dimension.Z/2);
            points[4] = new Vector3(-dimension.X/2, -dimension.Y/2, dimension.Z/2);
            points[5] = new Vector3(dimension.X/2, -dimension.Y/2, dimension.Z/2);
            points[6] = new Vector3(-dimension.X/2, dimension.Y/2, dimension.Z/2);
            points[7] = new Vector3(dimension.X/2, dimension.Y/2, dimension.Z/2);

            // Build axes for direction-local coordinate system.
            Vector3 nr1 = direction.Normalize(), nr2, nr3;
            nr1.GetPlanarOrthogonalVectors(out nr2, out nr3);

            // Transform the bounding box from local (direction-aligned) to the
            // absolute coordinate system and get the maximum extent for each axis.
            double diffX = 0, diffY = 0, diffZ = 0;


            foreach (Vector3 point in points) {
                double x = point.X*nr1.X + point.Y*nr1.Y + point.Z*nr1.Z;
                double y = point.X*nr2.X + point.Y*nr2.Y + point.Z*nr2.Z;
                double z = point.X*nr3.X + point.Y*nr3.Y + point.Z*nr3.Z;
                Vector3 temp = new Vector3(x, y, z);
                if (temp.X > diffX) {
                    diffX = temp.X;
                }
                if (temp.Y > diffY) {
                    diffY = temp.Y;
                }
                if (temp.Z > diffZ) {
                    diffZ = temp.Z;
                }
            }

            // Create axis-aligned bounding box (AABB) and assign values.
            return new AABB {
                XIntv = new AxisAlignedBoundingInterval(position.X - diffX, position.X + diffX),
                YIntv = new AxisAlignedBoundingInterval(position.Y - diffY, position.Y + diffY),
                ZIntv = new AxisAlignedBoundingInterval(position.Z - diffZ, position.Z + diffZ)
            };
        }

        public override string ToString() {
            return "X-Inv: " + XIntv + "\nY-Inv: " + YIntv + "\nZ-Inv: " + ZIntv;
        }

        public bool IntersectsWith( AABB aabb) {
            return XIntv.IntersectWith(aabb.XIntv) && YIntv.IntersectWith(aabb.YIntv) && ZIntv.IntersectWith(aabb.ZIntv);
        }

        public static bool Intersects(AABB a, AABB b) {
            return a.IntersectsWith(b);
        }

        #region Nested type: AxisAlignedBoundingInterval

        public class AxisAlignedBoundingInterval {
            public readonly double _min;
            public readonly double _max;

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
            public bool IntersectWith(AxisAlignedBoundingInterval other) {
                if (ReferenceEquals(this, other)) {
                    return true; //| Overlap with ...  
                }
                return (_min >= other._min && _min < other._max) || //| 1) left interval
                       (other._min >= _min && other._min < _max) || //| 2) right interval
                       (_min <= other._min && _max >= other._max) || //| 3) inner interval
                       (other._min <= _min && other._max >= _max); //| 4) outer interval.
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