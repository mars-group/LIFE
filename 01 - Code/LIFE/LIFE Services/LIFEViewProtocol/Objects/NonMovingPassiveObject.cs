using ProtoBuf;

namespace MessageWrappers {
	[ProtoContract]
	public class NonMovingPassiveObject : BasicPassiveObject {
		[ProtoMember(1)]
		public string Species { get; set; }

		[ProtoMember(2)]
		public double Height { get; set; }

		[ProtoMember(3)]
		public double Width { get; set; }

		[ProtoMember(4)]
		public double Depth { get; set; }

		protected NonMovingPassiveObject() {
			GetInheritancePath();
		}

		public NonMovingPassiveObject(Definitions.PassiveTypes objectType, double x, double y, double z, float rotation,
			string id, string description, double height, double width, double depth, string species = null)
			: base(objectType, x, y, z, rotation, id, description) {
			Species = species;
			Height = height;
			Width = width;
			Depth = depth;
		}
	}
}