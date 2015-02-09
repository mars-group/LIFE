using System;
using System.Collections.Generic;
using ProtoBuf;

namespace MessageWrappers {
	/// <summary>
	///     Helper class for storing the attributes of an agent
	/// </summary>
	[ProtoContract]
	public class AgentDefinition {
		[ProtoMember(1)]
		private Dictionary<string, Object> Attributes { get; set; }

		protected AgentDefinition() {}

		/// <summary>
		///     Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.
		/// </summary>
		public AgentDefinition(Dictionary<string, Object> attributes) {
			Attributes = attributes;
		}
	}
}