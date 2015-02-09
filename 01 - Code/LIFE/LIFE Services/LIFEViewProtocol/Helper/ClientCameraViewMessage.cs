using ProtoBuf;

namespace MessageWrappers {
	[ProtoContract]
	public class ClientCameraViewMessage : BasicVisualizationMessage {
		[ProtoMember(1)]
		public AreaDescriptor OldArea { get; set; }

		[ProtoMember(2)]
		public AreaDescriptor NewArea { get; set; }

		[ProtoMember(3)]
		public float NearClipping { get; set; }

		[ProtoMember(4)]
		public float FarClipping { get; set; }

		[ProtoMember(5)]
		public float CameraAngle { get; set; }

		[ProtoMember(6)]
		public Point3D CameraPosition { get; set; }
	}
}