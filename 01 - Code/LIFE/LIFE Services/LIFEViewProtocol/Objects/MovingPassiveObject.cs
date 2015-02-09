using ProtoBuf;

namespace MessageWrappers {
	[ProtoContract]
	public class MovingPassiveObject : BasicPassiveObject {
		[ProtoMember(1)]
		public string Species { get; set; }

		[ProtoMember(2)]
		public double Height { get; set; }

		[ProtoMember(3)]
		public double Width { get; set; }

		[ProtoMember(4)]
		public double Depth { get; set; }

		[ProtoMember(5)]
		public string Action { get; set; }

		[ProtoMember(6)]
		public string Type { get; set; }

		[ProtoMember(7)]
		public float Speed { get; set; }

		public MovingPassiveObject() {
			GetInheritancePath();
		}

		public MovingPassiveObject(Definitions.PassiveTypes objectType, double x, double y, double z, float rotation,
			string id, string description, float height, float width, double depth, string action, string type,
			float speed, string species = null) : base(objectType, x, y, z, rotation, id, description) {
			Species = species;
			Height = height;
			Depth = depth;
			Width = width;
			Action = action;
			Type = type;
			Speed = speed;
			GetInheritancePath();
		}
	}
}