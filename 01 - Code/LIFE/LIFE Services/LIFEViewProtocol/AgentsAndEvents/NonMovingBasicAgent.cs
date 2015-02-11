using System;
using System.Collections.Generic;
using LIFEViewProtocol.Basics;
using LIFEViewProtocol.Helper;
using ProtoBuf;

namespace LIFEViewProtocol.AgentsAndEvents {
	[ProtoContract]
	public class NonMovingBasicAgent : BasicAgent {
		[ProtoMember(1)]
		public string Species { get; set; }

		protected NonMovingBasicAgent() {
			GetInheritancePath();
		}

		/// <summary>
		///     Constructor for the BasicAgent
		/// </summary>
		/// <param name="agentType">Type of the agent. The types are listed in Definitions.AgentTypes</param>
		/// <param name="attributes">Dictionary of strings that contains relevant information</param>
		/// <param name="rotation"></param>
		/// <param name="id">The agent's ID</param>
		/// <param name="ticknumber">Ticknumber of the last update</param>
		/// <param name="x">Position in x-axis. May only be positive.</param>
		/// <param name="y">Position in y-axis. May only be positive.</param>
		/// <param name="z">Position in z-axis. May only be positive.</param>
		/// <param name="sizeX">Size modifier for the model in x-axis. For original size use 1.</param>
		/// <param name="sizeY">Size modifier for the model in y-axis. For original size use 1.</param>
		/// <param name="sizeZ">Size modifier for the model in z-axis. For original size use 1.</param>
		/// <param name="species">Species of the agent</param>
		/// <param name="groups">Groups of the agents. Optional parameter.</param>
		public NonMovingBasicAgent(Definitions.AgentTypes agentType, Dictionary<string, string> attributes, double x, double y, double z, float rotation, string id,
			long ticknumber, float sizeX, float sizeY, float sizeZ, string species, List<GroupDefinition> groups = null) : base(agentType, x, y, z, rotation, id, ticknumber, sizeX, sizeY, sizeZ, attributes, groups) {
			Species = species;
			GetInheritancePath();
		}
	}
}