using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCTestLayer
{
    public class Vector3f : IEquatable<Vector3f>
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public Vector3f(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public bool Equals(Vector3f other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Vector3f)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(this.Z * 397) ^ (int)(this.Y * 397) ^ (int)this.X;
            }
        }

        public static bool operator ==(Vector3f left, Vector3f right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Vector3f left, Vector3f right)
        {
            return !Equals(left, right);
        }

        public Vector3f Normalize()
        {
            float length = (float)Math.Sqrt(X * X + Y * Y + Z * Z);
            if (length == 0) length = 1;
            return new Vector3f(X / length, Y / length, Z / length);
        }

        public override string ToString()
        {
            return String.Format("({0}/{1}/{2})", X, Y, Z);
        }



        /// <summary>
        ///   Calculate point-to-point distance.
        /// </summary>
        /// <param name="pos">The target point.</param>
        /// <returns>Euclidian distance value.</returns>
        public float GetDistance(Vector3f pos) {
          return (float) Math.Sqrt((X - pos.X)*(X - pos.X) +
                                   (Y - pos.Y)*(Y - pos.Y) +
                                   (Z - pos.Z)*(Z - pos.Z));      
        }
    }
}
