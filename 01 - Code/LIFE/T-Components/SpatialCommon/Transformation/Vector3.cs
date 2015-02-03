using System;

namespace SpatialCommon.Transformation {

    public struct Vector3 : IEquatable<Vector3> {
        /// <summary>
        ///     Calculate the vector length.
        /// </summary>
        /// <returns>Length of this vector.</returns>
        public double Length { get { return GetDistance(Zero, this); } }

        public static Vector3 Random { get { return new Vector3(random.Next(), random.Next(), random.Next()); } }

        public static readonly Vector3 One = new Vector3(1.0d, 1.0d, 1.0d);
        public static readonly Vector3 Zero = new Vector3(0.0d, 0.0d, 0.0d);
        public static readonly Vector3 Forward = new Vector3(0.0d, 0.0d, 1.0d);
        public static readonly Vector3 Backward = new Vector3(0.0d, 0.0d, -1.0d);
        public static readonly Vector3 Up = new Vector3(0.0d, 1.0d, 0.0d);
        public static readonly Vector3 Down = new Vector3(0.0d, -1.0d, 0.0d);
        public static readonly Vector3 Left = new Vector3(-1.0d, 0.0d, 0.0d);
        public static readonly Vector3 Right = new Vector3(1.0d, 0.0d, 0.0d);
        public static readonly Vector3 Null = new Vector3(0.0d, 0.0d, 0.0d, true);
        public static readonly Vector3 UniVector3XAxis = new Vector3(1.0d, 0.0d, 0.0d).Normalize();
        public static readonly Vector3 MaxVector = new Vector3(double.MaxValue, double.MaxValue, double.MaxValue);

        private static readonly Random random = new Random();
        private readonly bool _is3D; // Dimension flag: false: 2D, true: 3D.
        private readonly bool _isSet;

        public readonly double X, Y, Z;


        /// <summary>
        ///     Initialize a two-dimensional vector (height is set to zero).
        /// </summary>
        public Vector3(double x, double y)
            : this(x, y, 0) {
            _is3D = false;
        }


        /// <summary>
        ///     Initialize a three-dimensional vector.
        /// </summary>
        public Vector3(double x, double y, double z) : this(x, y, z, true) {}

        /// <summary>
        ///     Initialize a three-dimensional vector.
        /// </summary>
        private Vector3(double x, double y, double z, bool isSet)
            : this() {
            X = x;
            Y = y;
            Z = z;
            _is3D = true;
            _isSet = isSet;
        }

        #region IEquatable<Vector3> Members

        public bool Equals(Vector3 other) {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && _isSet.Equals(other._isSet);
        }

        #endregion

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            return obj is Vector3 && Equals((Vector3) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode*397) ^ Y.GetHashCode();
                hashCode = (hashCode*397) ^ Z.GetHashCode();
                hashCode = (hashCode*397) ^ _isSet.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Vector3 left, Vector3 right) {
            return Equals(left, right);
        }

        public static bool operator !=(Vector3 left, Vector3 right) {
            return !Equals(left, right);
        }

        public Vector3 Normalize() {
            double length = Math.Sqrt(X*X + Y*Y + Z*Z);
            if (length <= double.Epsilon) {
                length = 1;
            }
            return new Vector3(X/length, Y/length, Z/length);
        }

        public Vector3 Normalize(double length) {
            if (Math.Abs(length) < 0.0001) {
                return Zero;
            }

            double cur = ComputeMagnitude();
            if (Math.Abs(cur) < 0.0000001) {
                throw new Exception("Attempting to normalize a zero vector");
            }

            double coeff = length/cur;
            return new Vector3(X*coeff, Y*coeff, Z*coeff);
        }

        /// <summary>
        ///     Output the position.
        /// </summary>
        /// <returns>String with component-based notation.</returns>
        public override string ToString() {
            return !_is3D
                ? String.Format("({0,5:0.00}|{1,5:0.00})", X, Y)
                : String.Format("({0,5:0.00}|{1,5:0.00}|{2,5:0.00})", X, Y, Z);
        }


        /// <summary>
        ///     Calculate point-to-point distance.
        /// </summary>
        /// <param name="pos">The target point.</param>
        /// <returns>Euclidian distance value.</returns>
        public double GetDistance(Vector3 pos) {
            return
                (Math.Sqrt
                    (Math.Pow(Math.Abs(X - pos.X), 2) + Math.Pow(Math.Abs(Y - pos.Y), 2)
                     + Math.Pow(Math.Abs(Z - pos.Z), 2)));
        }

        /// <summary>
        ///     Calculate point-to-point distance.
        /// </summary>
        /// <param name="from">The first point.</param>
        /// <param name="to">The second point.</param>
        /// <returns>Euclidian distance value.</returns>
        public static double GetDistance(Vector3 @from, Vector3 to) {
            return
                (Math.Sqrt
                    (Math.Pow(Math.Abs(@from.X - to.X), 2) + Math.Pow(Math.Abs(@from.Y - to.Y), 2)
                     + Math.Pow(Math.Abs(@from.Z - to.Z), 2)));
        }

        /// <summary>
        ///     Create normalized vectors orthogonal to this one.
        /// </summary>
        /// <param name="nY">Pointer for new y-axis normal vector.</param>
        /// <param name="nZ">Same for z-axis (height) vector.</param>
        public void GetPlanarOrthogonalVectors(out Vector3 nY, out Vector3 nZ) {
            // [Y-Axis]: Create orthogonal vector to new x-axis laying in plane (x, y): => Scalar product = 0.
            nY = (Math.Abs(X) <= double.Epsilon) ? UniVector3XAxis : new Vector3(-Y/X, 1.0d, 0.0d).Normalize();

            // [Z-Axis / Height]: Build orthogonal vector with cross-product.
            double x3 = (Y*nY.Z - Z*nY.Y); // x: a2b3 - a3b2
            double y3 = (Z*nY.X - X*nY.Z); // y: a3b1 - a1b3
            double z3 = (X*nY.Y - Y*nY.X); // z: a1b2 - a2b1
            nZ = new Vector3(x3, y3, z3).Normalize();

            //Console.WriteLine("GPO: NX: "+this.ToString());
            //Console.WriteLine("GPO: NY: "+nY);
            //Console.WriteLine("GPO: NZ: "+nZ+"\n");
        }

        public bool IsNull() {
            return !_isSet;
        }

        #region additionalMethods

        // Finds the square of the magnitude of the 3D vector (the square root operation is done after this, so this may be all that's needed)
        public double ComputeMagnitudeSquared() {
            return X*X + Y*Y + Z*Z;
        }

        // Finds the magnitude of the 3D vector
        public double ComputeMagnitude() {
            return Math.Sqrt(ComputeMagnitudeSquared());
        }

        public static bool operator <=(Vector3 first, Vector3 second) {
            return first.X <= second.X & first.Y <= second.Y & first.Z <= second.Z;
        }

        public static bool operator >=(Vector3 first, Vector3 second) {
            return first.X >= second.X & first.Y >= second.Y & first.Z >= second.Z;
        }

        // The sum of a pair of 3D vectors
        public static Vector3 operator +(Vector3 left, Vector3 right) {
            return new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        // The additive inverse of a vector
        public static Vector3 operator -(Vector3 right) {
            return new Vector3(-right.X, -right.Y, -right.Z);
        }

        // The difference between 3D vectors
        public static Vector3 operator -(Vector3 left, Vector3 right) {
            return new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        // Scalar product with a 3D vector
        public static Vector3 operator *(Vector3 left, double right) {
            return new Vector3(left.X*right, left.Y*right, left.Z*right);
        }

        public static Vector3 operator *(double left, Vector3 right) {
            return new Vector3(right.X*left, right.Y*left, right.Z*left);
        }

        // Scalar division
        public static Vector3 operator /(Vector3 left, double right) {
            return left*(1.0f/right);
        }

        // The cross product of a pair of 3D vectors
        public static Vector3 Cross(Vector3 a, Vector3 b) {
            return new Vector3(a.Y*b.Z - a.Z*b.Y, a.Z*b.X - a.X*b.Z, a.X*b.Y - a.Y*b.X);
        }

        public static Vector3 Cross(ref Vector3 a, ref Vector3 b) {
            return new Vector3(a.Y*b.Z - a.Z*b.Y, a.Z*b.X - a.X*b.Z, a.X*b.Y - a.Y*b.X);
        }


        /// <summary>
        ///     Returns the squared length of the vector.
        /// </summary>
        /// <returns>The squared length of the vector. (X*X + Y*Y + Z*Z)</returns>
        public double GetLengthSquared() {
            return (X*X + Y*Y + Z*Z);
        }

        #endregion
    }

}