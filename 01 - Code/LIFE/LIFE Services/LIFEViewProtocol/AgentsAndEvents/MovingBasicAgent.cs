using System.Collections.Generic;
using LIFEViewProtocol.Basics;
using LIFEViewProtocol.Helper;
using ProtoBuf;

namespace LIFEViewProtocol.AgentsAndEvents {
	[ProtoContract]
	public class MovingBasicAgent : BasicAgent {
		[ProtoMember(1)]
		public string Species { get; set; }

		[ProtoMember(2)]
		public string Action { get; set; }

		[ProtoMember(3)]
		public float Speed { get; set; }

		protected MovingBasicAgent() {
			GetInheritancePath();
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
		/// <param name="speed">Speed of the agent</param>
		/// <param name="action">Current action of the agent. Can be null.</param>
		/// <param name="species">Species of the agent. Can be null.</param>
		/// <param name="groups">The groups this agent is assigned to. Can be null</param>
		public MovingBasicAgent(Definitions.AgentTypes type, Dictionary<string, string> attributes, double x, double y, double z, float rotation, string id, long ticknumber, float sizeX, float sizeY, float sizeZ, float speed, string action = null, string species = null, List<GroupDefinition> groups = null)
			: base(type, x, y, z, rotation, id, ticknumber, sizeX, sizeY, sizeZ, attributes, groups) {
			Species = species;
			Action = action;
			Speed = speed;
			GetInheritancePath();
		}
	}
}