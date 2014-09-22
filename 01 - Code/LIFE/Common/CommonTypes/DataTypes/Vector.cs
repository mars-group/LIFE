using System;

namespace CommonTypes.DataTypes {
    public struct Vector : IEquatable<Vector> {
        public static readonly Vector Origin = new Vector(0.0f, 0.0f, 0.0f);
        public static readonly Vector Null = new Vector(0.0f, 0.0f, 0.0f, true);
        public static readonly Vector UnitVectorXAxis = new Vector(1.0f, 0.0f, 0.0f).Normalize();
        public static readonly Vector MaxVector = new Vector(float.MaxValue, float.MaxValue, float.MaxValue);
       
        private readonly bool _is3D; // Dimension flag: false: 2D, true: 3D.
        private readonly bool _isNull;

        public float X, Y, Z;


        /// <summary>
        ///     Initialize a two-dimensional vector (height is set to zero).
        /// </summary>
        public Vector(float x, float y) : this(x, y, 0) {
            _is3D = false;
        }


        /// <summary>
        ///     Initialize a three-dimensional vector.
        /// </summary>
        public Vector(float x, float y, float z) : this(x, y, z, false) {}

        /// <summary>
        ///     Initialize a three-dimensional vector.
        /// </summary>
        private Vector(float x, float y, float z, bool isNull) : this() {
            X = x;
            Y = y;
            Z = z;
            _is3D = true;
            _isNull = isNull;
        }

        #region IEquatable<Vector> Members

        public bool Equals(Vector other) {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && _isNull.Equals(other._isNull);
        }

        #endregion

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector && Equals((Vector) obj);
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

        public static bool operator ==(Vector left, Vector right) {
            return Equals(left, right);
        }

        public static bool operator !=(Vector left, Vector right) {
            return !Equals(left, right);
        }

        public Vector Normalize() {
            float length = (float) Math.Sqrt(X*X + Y*Y + Z*Z);
            if (length <= float.Epsilon) length = 1;
            return new Vector(X/length, Y/length, Z/length);
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
        public float GetDistance(Vector pos) {
            return (float) Math.Sqrt
                ((X - pos.X)*(X - pos.X) +
                 (Y - pos.Y)*(Y - pos.Y) +
                 (Z - pos.Z)*(Z - pos.Z));
        }


        /// <summary>
        ///     Create normalized vectors orthogonal to this one.
        /// </summary>
        /// <param name="nY">Pointer for new y-axis normal vector.</param>
        /// <param name="nZ">Same for z-axis (height) vector.</param>
        public void GetPlanarOrthogonalVectors(out Vector nY, out Vector nZ) {
            // [Y-Axis]: Create orthogonal vector to new x-axis laying in plane (x, y): => Scalar product = 0.
            nY = (Math.Abs(X) <= float.Epsilon) ? UnitVectorXAxis : new Vector(-Y/X, 1.0f, 0.0f).Normalize();

            // [Z-Axis / Height]: Build orthogonal vector with cross-product.
            float x3 = (Y*nY.Z - Z*nY.Y); // x: a2b3 - a3b2
            float y3 = (Z*nY.X - X*nY.Z); // y: a3b1 - a1b3
            float z3 = (X*nY.Y - Y*nY.X); // z: a1b2 - a2b1
            nZ = new Vector(x3, y3, z3).Normalize();

            //Console.WriteLine("GPO: NX: "+this.ToString());
            //Console.WriteLine("GPO: NY: "+nY);
            //Console.WriteLine("GPO: NZ: "+nZ+"\n");
        }
    }
}