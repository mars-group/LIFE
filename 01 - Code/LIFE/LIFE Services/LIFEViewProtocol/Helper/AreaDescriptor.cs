using System;
using System.Collections.Generic;
using ProtoBuf;

namespace LIFEViewProtocol.Helper {
	[ProtoContract]
	public class AreaDescriptor : IEquatable<AreaDescriptor> {
		[ProtoMember(1)]
		public Point3D TopLeft { get; set; }

		[ProtoMember(2)]
		public Point3D TopRight { get; set; }

		[ProtoMember(3)]
		public Point3D BottomRight { get; set; }

		[ProtoMember(4)]
		public Point3D BottomLeft { get; set; }

		[ProtoMember(5)]
		public string Guid { get; set; }

		public AreaDescriptor(Point3D topLeft, Point3D topRight, Point3D bottomRight, Point3D bottomLeft) {
			TopLeft = topLeft;
			TopRight = topRight;
			BottomRight = bottomRight;
			BottomLeft = bottomLeft;
		}

		public double[] GetXPoints() {
			return new[] {TopLeft.X, TopRight.X, BottomRight.X, BottomLeft.X};
		}

		public double[] GetZPoints() {
			return new[] {TopLeft.Z, TopRight.Z, BottomRight.Z, BottomLeft.Z};
		}

		public double[] GetYPoints() {
			return new[] {TopLeft.Y, TopRight.Y, BottomRight.Y, BottomLeft.Y};
		}

		public double GetHighestX() {
			return Math.Max(TopLeft.X, Math.Max(TopRight.X, Math.Max(BottomRight.X, BottomLeft.X)));
		}

		public double GetLowestX() {
			return Math.Min(TopLeft.X, Math.Min(TopRight.X, Math.Min(BottomRight.X, BottomLeft.X)));
		}

		public double GetHighestZ() {
			return Math.Max(TopLeft.Z, Math.Max(TopRight.Z, Math.Max(BottomRight.Z, BottomLeft.Z)));
		}

		public double GetLowestZ() {
			return Math.Min(TopLeft.Z, Math.Min(TopRight.Z, Math.Min(BottomRight.Z, BottomLeft.Z)));
		}

		public double GetHighestY() {
			return Math.Max(TopLeft.Y, Math.Max(TopRight.Y, Math.Max(BottomRight.Y, BottomLeft.Y)));
		}

		public double GetLowestY() {
			return Math.Min(TopLeft.Y, Math.Min(TopRight.Y, Math.Min(BottomRight.Y, BottomLeft.Y)));
		}

		public List<Point3D> GetAllPoints() {
			return new List<Point3D> {TopLeft, TopRight, BottomRight, BottomLeft};
		}

		public AreaDescriptor() {}

		#region IEquatable<AreaDescriptor> Members

		public bool Equals(AreaDescriptor other) {
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(TopLeft, other.TopLeft) && Equals(TopRight, other.TopRight) && Equals(BottomRight, other.BottomRight)
			       && Equals(BottomLeft, other.BottomLeft);
		}

		#endregion

		public override int GetHashCode() {
			unchecked {
				int hashCode = (TopLeft != null ? TopLeft.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (TopRight != null ? TopRight.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (BottomRight != null ? BottomRight.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (BottomLeft != null ? BottomLeft.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(AreaDescriptor left, AreaDescriptor right) {
			return Equals(left, right);
		}

		public static bool operator !=(AreaDescriptor left, AreaDescriptor right) {
			return !Equals(left, right);
		}

		public override bool Equals(object o) {
			if (ReferenceEquals(null, o)) return false;
			if (ReferenceEquals(this, o)) return true;
			if (o.GetType() != GetType()) return false;
			return Equals((AreaDescriptor) o);
		}

		public override string ToString() {
			return "TL: " + TopLeft + "TR: " + TopRight + " BR: " + BottomRight + " BL: " + BottomLeft;
		}

		public bool Equals(Point3D tl, Point3D tr, Point3D br, Point3D bl) {
			if (TopLeft != tl || TopRight != tr || BottomRight != br || BottomLeft != bl) return false;
			return true;
		}
	}
}