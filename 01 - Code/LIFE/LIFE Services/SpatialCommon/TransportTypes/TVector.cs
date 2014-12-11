using System;
using SpatialCommon.Datatypes;

namespace SpatialCommon.TransportTypes {

    public struct TVector : IEquatable<TVector> {
        public static readonly TVector Origin = new TVector(0.0d, 0.0d, 0.0d);
        public static readonly TVector Null = new TVector(0.0d, 0.0d, 0.0d, true);
        public static readonly TVector UnitVectorXAxis = new TVector(1.0d, 0.0d, 0.0d).Normalize();
        public static readonly TVector MaxVector = new TVector(double.MaxValue, double.MaxValue, double.MaxValue);

        private readonly bool _is3D; // Dimension flag: false: 2D, true: 3D.
        private readonly bool _isNull;

        public readonly double X, Y, Z;


        /// <summary>
        ///     Initialize a two-dimensional vector (height is set to zero).
        /// </summary>
        public TVector(double x, double y) : this(x, y, 0) {
            _is3D = false;
        }


        /// <summary>
        ///     Initialize a three-dimensional vector.
        /// </summary>
        public TVector(double x, double y, double z) : this(x, y, z, false) {}

        /// <summary>
        ///     Initialize a three-dimensional vector.
        /// </summary>
        private TVector(double x, double y, double z, bool isNull) : this() {
            X = x;
            Y = y;
            Z = z;
            _is3D = true;
            _isNull = isNull;
        }

        #region IEquatable<TVector> Members

        public bool Equals(TVector other) {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && _isNull.Equals(other._isNull);
        }

        #endregion

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TVector && Equals((TVector) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode*397) ^ Y.GetHashCode();
                hashCode = (hashCode*397) ^ Z.GetHashCode();
                hashCode = (hashCode*397) ^ _isNull.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(TVector left, TVector right) {
            return Equals(left, right);
        }

        public static bool operator !=(TVector left, TVector right) {
            return !Equals(left, right);
        }

        public TVector Normalize() {
            double length = Math.Sqrt(X*X + Y*Y + Z*Z);
            if (length <= double.Epsilon) length = 1;
            return new TVector(X/length, Y/length, Z/length);
        }

        public TVector Normalize(double length) {
            if (Math.Abs(length) < 0.0001)
                return Origin;

            double cur = ComputeMagnitude();
            if (Math.Abs(cur) < 0.0000001)
                throw new Exception("Attempting to normalize a zero vector");

            double coeff = length/cur;
            return new TVector(X*coeff, Y*coeff, Z*coeff);
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
        public double GetDistance(TVector pos) {
            return Math.Sqrt
                ((X - pos.X)*(X - pos.X) +
                 (Y - pos.Y)*(Y - pos.Y) +
                 (Z - pos.Z)*(Z - pos.Z));
        }


        /// <summary>
        ///     Create normalized vectors orthogonal to this one.
        /// </summary>
        /// <param name="nY">Pointer for new y-axis normal vector.</param>
        /// <param name="nZ">Same for z-axis (height) vector.</param>
        public void GetPlanarOrthogonalVectors(out TVector nY, out TVector nZ) {
            // [Y-Axis]: Create orthogonal vector to new x-axis laying in plane (x, y): => Scalar product = 0.
            nY = (Math.Abs(X) <= double.Epsilon) ? UnitVectorXAxis : new TVector(-Y/X, 1.0d, 0.0d).Normalize();

            // [Z-Axis / Height]: Build orthogonal vector with cross-product.
            double x3 = (Y*nY.Z - Z*nY.Y); // x: a2b3 - a3b2
            double y3 = (Z*nY.X - X*nY.Z); // y: a3b1 - a1b3
            double z3 = (X*nY.Y - Y*nY.X); // z: a1b2 - a2b1
            nZ = new TVector(x3, y3, z3).Normalize();

            //Console.WriteLine("GPO: NX: "+this.ToString());
            //Console.WriteLine("GPO: NY: "+nY);
            //Console.WriteLine("GPO: NZ: "+nZ+"\n");
        }

        public bool IsNull() {
            return _isNull;
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

        public static double Dot(TVector a, TVector b) {
            return a.X*b.X + a.Y*b.Y + a.Z*b.Z;
        }

        public static double Dot(ref TVector a, ref TVector b) {
            return a.X*b.X + a.Y*b.Y + a.Z*b.Z;
        }

        // The sum of a pair of 3D vectors
        public static TVector operator +(TVector left, TVector right) {
            return new TVector(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        // The additive inverse of a vector
        public static TVector operator -(TVector right) {
            return new TVector(-right.X, -right.Y, -right.Z);
        }

        // The difference between 3D vectors
        public static TVector operator -(TVector left, TVector right) {
            return new TVector(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        // Scalar product with a 3D vector
        public static TVector operator *(TVector left, double right) {
            return new TVector(left.X*right, left.Y*right, left.Z*right);
        }

        public static TVector operator *(double left, TVector right) {
            return new TVector(right.X*left, right.Y*left, right.Z*left);
        }

        // Scalar division
        public static TVector operator /(TVector left, double right) {
            return left*(1.0f/right);
        }

        // The cross product of a pair of 3D vectors
        public static TVector Cross(TVector a, TVector b) {
            return new TVector(a.Y*b.Z - a.Z*b.Y, a.Z*b.X - a.X*b.Z, a.X*b.Y - a.Y*b.X);
        }

        public static TVector Cross(ref TVector a, ref TVector b) {
            return new TVector(a.Y*b.Z - a.Z*b.Y, a.Z*b.X - a.X*b.Z, a.X*b.Y - a.Y*b.X);
        }


        /// <summary>
        ///     Returns the squared length of the vector.
        /// </summary>
        /// <returns>The squared length of the vector. (X*X + Y*Y + Z*Z)</returns>
        public double GetLengthSquared() {
            return (X*X + Y*Y + Z*Z);
        }

        #endregion

        /// <summary>
        ///   Casts this TVector to a Vector object.
        /// </summary>
        /// <returns>Vector equivalent.</returns>
        public Vector GetVector()
        {
            return new Vector(X, Y, Z);
        }
    }
}