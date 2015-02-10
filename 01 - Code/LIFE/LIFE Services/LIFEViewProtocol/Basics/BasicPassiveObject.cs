using GeoAPI.Geometries;
using LIFEViewProtocol.Helper;
using LIFEViewProtocol.Objects;
using ProtoBuf;

namespace LIFEViewProtocol.Basics {
	/// <summary>
	///     Basic type for passive, non-simulated objects which are part of the simulation like buildings or vegetation.
	/// </summary>
	[ProtoContract]
	[ProtoInclude(100, typeof (MovingPassiveObject))]
	[ProtoInclude(101, typeof (NonMovingPassiveObject))]
	[ProtoInclude(102, typeof (PrimitiveObject))]
	public class BasicPassiveObject : BasicVisualizationMessage {
		[ProtoMember(1)] public Definitions.PassiveTypes ObjectType;

		[ProtoMember(2)]
		public double X { get; set; }

		[ProtoMember(3)]
		public double Y { get; set; }

		[ProtoMember(4)]
		public double Z { get; set; }

		[ProtoMember(5)]
		public float Rotation { get; set; }

		[ProtoMember(6)]
		public string Id { get; set; }

		/// <summary>
		///     Use this to specify the object (eg. male, female, young, old...).
		///     Will be displayed on inspection and may also be used to give different looks to the objects.
		/// </summary>
		[ProtoMember(7)]
		public string Description { get; set; }

		protected BasicPassiveObject() {
			GetInheritancePath();
		}

		/// <summary>
		///     Returns a coordinate in GeoAPI-Format
		/// </summary>
		/// <returns></returns>
		public Coordinate GetCoordinate() {
			return new Coordinate(X, Y, Z);
		}

		public BasicPassiveObject(Definitions.PassiveTypes objectType, double x, double y, double z, float rotation, string id,
			string description = null) {
			ObjectType = objectType;
			X = x;
			Y = y;
			Z = z;
			Rotation = rotation;
			Id = id;
			Description = description;
			GetInheritancePath();
		}
	}
}