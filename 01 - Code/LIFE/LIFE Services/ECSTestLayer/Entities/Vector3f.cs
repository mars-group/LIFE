﻿using System;

namespace ESCTestLayer.Entities
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

        public override string ToString() {
            return String.Format("({0,5:0.00}|{1,5:0.00}|{2,5:0.00})", X, Y, Z);
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


        /// <summary>
        ///   Create normalized vectors orthogonal to this one.
        /// </summary>
        /// <param name="nY">Pointer for new y-axis normal vector.</param>
        /// <param name="nZ">Same for z-axis (height) vector.</param>
        public void GetPlanarOrthogonalVectors(out Vector3f nY, out Vector3f nZ) {
      
          // [Y-Axis]: Create orthogonal vector to new x-axis laying in plane (x, y): => Scalar product = 0.
          nY = (Math.Abs(X) <= float.Epsilon)? new Vector3f(1.0f, 0.0f, 0.0f).Normalize(): 
                                               new Vector3f(-Y/X, 1.0f, 0.0f).Normalize();
          
          // [Z-Axis / Height]: Build orthogonal vector with cross-product.
          var x3 = (Y * nY.Z  -  Z * nY.Y);  // x: a2b3 - a3b2
          var y3 = (Z * nY.X  -  X * nY.Z);  // y: a3b1 - a1b3
          var z3 = (X * nY.Y  -  Y * nY.X);  // z: a1b2 - a2b1
          nZ = new Vector3f(x3, y3, z3).Normalize();

          //Console.WriteLine("GPO: NX: "+this.ToString());
          //Console.WriteLine("GPO: NY: "+nY);
          //Console.WriteLine("GPO: NZ: "+nZ+"\n");
        }
    }
}