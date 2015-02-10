using System.Collections.Generic;
using LIFEViewProtocol.Basics;
using LIFEViewProtocol.Helper;
using LIFEViewProtocol.Terrain;
using ProtoBuf;

namespace LIFEViewProtocol.Actions {
	/// <summary>
	/// </summary>
	[ProtoContract]
	public class InitializationMessage : BasicVisualizationMessage {
		[ProtoMember(1)]
		public TerrainDataMessage Terrain { get; private set; }

		[ProtoMember(2)]
		public Dictionary<Definitions.AgentTypes, Definitions.AgentColors> ExpectedAgents { get; private set; }

		[ProtoMember(3)]
		public Dictionary<string, GroupDefinition> ExpectedGroups { get; private set; }

		[ProtoMember(4)]
		public Dictionary<string, string> ExpectedLayer { get; private set; }

		[ProtoMember(5)]
		public bool Transactional { get; private set; }

		/// <summary>
		/// </summary>
		protected InitializationMessage() {
			GetInheritancePath();
		}

		/// <summary>
		/// </summary>
		/// <param name="terrain"></param>
		/// <param name="expectedAgents"></param>
		public InitializationMessage(TerrainDataMessage terrain,
			Dictionary<Definitions.AgentTypes, Definitions.AgentColors> expectedAgents,
			Dictionary<string, GroupDefinition> expectedGroups, Dictionary<string, string> expectedLayer,
			bool transactional = false) {
			Terrain = terrain;
			ExpectedAgents = expectedAgents;
			ExpectedGroups = expectedGroups;
			Transactional = transactional;
			GetInheritancePath();
		}
	}
}