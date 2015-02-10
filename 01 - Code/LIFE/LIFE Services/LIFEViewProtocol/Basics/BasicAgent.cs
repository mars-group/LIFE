using System.Collections.Generic;
using GeoAPI.Geometries;
using LIFEViewProtocol.AgentsAndEvents;
using LIFEViewProtocol.Helper;
using ProtoBuf;

namespace LIFEViewProtocol.Basics {
	[ProtoContract]
	[ProtoInclude(100, typeof (NonMovingBasicAgent))]
	[ProtoInclude(101, typeof (MovingBasicAgent))]
	public abstract class BasicAgent : BasicVisualizationMessage {
		[ProtoMember(1)]
		public double X { get; set; }

		[ProtoMember(2)]
		public double Y { get; set; }

		[ProtoMember(3)]
		public double Z { get; set; }

		[ProtoMember(4)]
		public float Rotation { get; set; }

		[ProtoMember(5)]
		public string Id { get; set; }

		/// <summary>
		///     State of the agent (eg. Dead, Alive, Active, Passive). Not to confuse with its current action.
		/// </summary>
		[ProtoMember(6)]
		public string State { get; set; }

		[ProtoMember(7)]
		public Dictionary<string, string> Attributes { get; set; }

		[ProtoMember(8)]
		public float Size_X { get; set; }

		[ProtoMember(9)]
		public float Size_Y { get; set; }

		[ProtoMember(10)]
		public float Size_Z { get; set; }

		[ProtoMember(11)]
		public Definitions.AgentTypes AgentType { get; set; }

		[ProtoMember(12)]
		public List<GroupDefinition> Groups { get; set; }

		protected BasicAgent() {
			GetInheritancePath();
		}

		public Coordinate GetCoordinate() {
			return new Coordinate(X, Y, Z);
		}

		/// <summary>
		///     Constructor for the BasicAgent
		/// </summary>
		/// <param name="type">Type of the agent. The types are listed in Definitions.AgentTypes</param>
		/// <param name="x">Position X</param>
		/// <param name="y">Position Y</param>
		/// <param name="z">Position Z</param>
		/// <param name="rotation">Rotation in degree</param>
		/// <param name="id">The agent's ID</param>
		/// <param name="ticknumber">Ticknumber of the last update</param>
		/// <param name="sizeX">Width of the agent's model</param>
		/// <param name="sizeY">Height of the agent's model</param>
		/// <param name="sizeZ">Depth of the agent's model</param>
		/// <param name="attributes">Dictionary of strings that contains relevant information</param>
		/// <param name="groups">The groups this agent is assigned to</param>
		public BasicAgent(Definitions.AgentTypes type, double x, double y, double z, float rotation, string id,
			long ticknumber, float sizeX, float sizeY, float sizeZ, Dictionary<string, string> attributes,
			List<GroupDefinition> groups = null) {
			AgentType = type;
			X = x;
			Y = y;
			Z = z;
			Rotation = rotation;
			Id = id;
			TickNumber = ticknumber;
			Size_X = sizeX;
			Size_Y = sizeY;
			Size_Z = sizeZ;
			Attributes = attributes;
			GetInheritancePath();
		}


		/// <summary>
		///     Constructor for the BasicAgent
		/// </summary>
		/// <param name="agentType">Type of the agent. The types are listed in Definitions.AgentTypes</param>
		/// <param name="attributes">Dictionary of strings that contains relevant information</param>
		/// <param name="id">The agent's ID</param>
		/// <param name="ticknumber">Ticknumber of the last update</param>
		/// <param name="groups">The groups this agent is assigned to</param>
		public BasicAgent(Definitions.AgentTypes agentType, Dictionary<string, string> attributes, string id, long ticknumber,
			List<GroupDefinition> groups) : this(agentType, 0, 0, 0, 0, id, ticknumber, 1, 1, 1, attributes) {
			Groups = groups;
		}

		/// <summary>
		///     Constructor for the BasicAgent
		/// </summary>
		/// <param name="agentType">Type of the agent. The types are listed in Definitions.AgentTypes</param>
		/// <param name="attributes">Dictionary of strings that contains relevant information</param>
		/// <param name="id">The agent's ID</param>
		/// <param name="ticknumber">Ticknumber of the last update</param>
		public BasicAgent(Definitions.AgentTypes agentType, Dictionary<string, string> attributes, string id, long ticknumber)
			:
				this(agentType, 0, 0, 0, 0, id, ticknumber, 1, 1, 1, attributes) {}
	}
}