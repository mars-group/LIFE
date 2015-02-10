using System;
using GeoAPI.Geometries;
using ProtoBuf;

namespace LIFEViewProtocol.Helper {
	[ProtoContract]
	public class Point3D : IEquatable<Point3D>, IComparable {
		[ProtoMember(1)]
		public double X { get; set; }

		[ProtoMember(2)]
		public double Y { get; set; }

		[ProtoMember(3)]
		public double Z { get; set; }

/*		public Point3D(double x, double y) {
			X = x;
			Y = y;
			Z = 0;
		}*/

		protected Point3D() {}

		public Point3D(double x = 0, double y = 0, double z = 0) {
			X = x;
			Y = y;
			Z = z;
		}

		/// <summary>
		///     Constructor for usage with a GeoAPI-Coordinate
		/// </summary>
		/// <param name="coordinate"></param>
		public Point3D(Coordinate coordinate) {
			X = coordinate.X;
			Y = coordinate.Y;
			Z = coordinate.Z;
		}

		#region IComparable Members

		public int CompareTo(object obj) {
			Point3D mp = obj as Point3D;

			if (mp != null && (X > mp.X || Y > mp.Y)) return 1;
			if (mp != null && (X < mp.X && Y < mp.Y)) return -1;
			return 0;
		}

		#endregion

		#region IEquatable<Point3D> Members

		public bool Equals(Point3D other) {
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
		}

		#endregion

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((Point3D) obj);
		}

		public static bool operator ==(Point3D left, Point3D right) {
			return Equals(left, right);
		}

		public static bool operator !=(Point3D left, Point3D right) {
			return !Equals(left, right);
		}

		public override int GetHashCode() {
			unchecked {
				int hashCode = X.GetHashCode();
				hashCode = (hashCode*397) ^ Y.GetHashCode();
				hashCode = (hashCode*397) ^ Z.GetHashCode();
				return hashCode;
			}
		}

		public override string ToString() {
			return ("X: " + X + ", Y: " + Y);
		}
	}
}