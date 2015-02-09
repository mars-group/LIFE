using System.Collections.Generic;
using GeoAPI.Geometries;
using MessageWrappers.Basics;
using NetTopologySuite.Geometries;
using ProtoBuf;

namespace MessageWrappers {
	[ProtoContract]
	public class Polygon : BasicVisualizationMessage {
		[ProtoMember(1)]
		private List<Point3D> Points { get; set; }

		private NetTopologySuite.Geometries.Polygon NtsPolygon { get; set; }


		protected Polygon() {
			var sequence = new List<Coordinate>();
			foreach (var point in Points) {
				sequence.Add(new Coordinate(point.X, point.Y, point.Z));
			}
			NtsPolygon = new NetTopologySuite.Geometries.Polygon(new LinearRing(sequence.ToArray()));
		}

		public Polygon(IPolygon poly) : this(poly.ExteriorRing.Coordinates) {}


		public Polygon(IEnumerable<Coordinate> coordinates) {
			Points = new List<Point3D>();
			foreach (var coordinate in coordinates) {
				Points.Add(new Point3D(coordinate));
			}
			GetInheritancePath();
		}
	}
}