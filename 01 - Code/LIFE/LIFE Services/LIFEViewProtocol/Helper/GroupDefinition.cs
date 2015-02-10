using System.Collections.Generic;
using LIFEViewProtocol.Basics;
using ProtoBuf;

namespace LIFEViewProtocol.Helper {
	[ProtoContract]
	public class GroupDefinition : BasicVisualizationMessage {
		[ProtoMember(1)]
		public int[] GroupColor { get; set; }

		[ProtoMember(2)]
		public string GroupName { get; set; }

		[ProtoMember(3)]
		public Dictionary<string, string> GroupDetails { get; set; }

		protected GroupDefinition() {}

		public GroupDefinition(int[] groupColor, string groupName) {
			GroupColor = groupColor;
			GroupName = groupName;
		}

		public GroupDefinition(int[] groupColor, string groupName, Dictionary<string, string> groupDetails) {
			GroupColor = groupColor;
			GroupName = groupName;
			GroupDetails = groupDetails;
		}
	}
}