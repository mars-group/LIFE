using System.Collections.Generic;
using LIFEViewProtocol.Basics;
using LIFEViewProtocol.Helper;
using ProtoBuf;

namespace LIFEViewProtocol.AgentsAndEvents {
	[ProtoContract]
	public class EnvironmentEvent : BasicVisualizationMessage {
		[ProtoMember(1)]
		public Definitions.EnvironmentEvents envEvent { get; private set; }

		[ProtoMember(2)]
		public double X { get; private set; }

		[ProtoMember(3)]
		public double Y { get; private set; }

		[ProtoMember(4)]
		public double Z { get; private set; }

		[ProtoMember(5)]
		public string Id { get; private set; }

		[ProtoMember(6)]
		public float Size_X { get; private set; }

		[ProtoMember(7)]
		public float Size_Y { get; private set; }

		[ProtoMember(8)]
		public float Size_Z { get; private set; }

		[ProtoMember(9)]
		public Dictionary<string, string> Attributes { get; private set; }

		protected EnvironmentEvent() {
			GetInheritancePath();
		}

		public EnvironmentEvent(Definitions.EnvironmentEvents envEvent, double x, double y, double z, string id,
			long ticknumber, float sizeX = 1, float sizeY = 1, float sizeZ = 1, Dictionary<string, string> attributes = null) {
			this.envEvent = envEvent;
			X = x;
			Y = y;
			Z = z;
			Id = id;
			TickNumber = ticknumber;
			Size_X = sizeX;
			Size_Y = sizeY;
			Size_Z = sizeZ;
			Attributes = attributes;
			GetInheritancePath();
		}
	}
}