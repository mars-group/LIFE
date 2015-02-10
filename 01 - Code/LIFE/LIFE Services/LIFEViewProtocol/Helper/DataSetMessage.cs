﻿using System.Collections.Generic;
using MessageWrappers.AgentsAndEvents;
using MessageWrappers.Basics;
using MessageWrappers.Objects;
using ProtoBuf;

namespace MessageWrappers.Helper {
	[ProtoContract]
	public class DataSetMessage : BasicVisualizationMessage {
		[ProtoMember(1)]
		public List<MovingBasicAgent> MovingBasicAgents { get; set; }

		[ProtoMember(2)]
		public List<NonMovingBasicAgent> NonMovingBasicAgents { get; set; }

		[ProtoMember(3)]
		public List<BasicAgent> BasicAgents { get; set; }

		[ProtoMember(4)]
		public List<MovingPassiveObject> MovingPassiveObjects { get; set; }

		[ProtoMember(5)]
		public List<NonMovingPassiveObject> NonMovingPassiveObjects { get; set; }

		[ProtoMember(6)]
		public List<BasicPassiveObject> BasicPassiveObjects { get; set; }

		public DataSetMessage() {
			GetInheritancePath();
		}
	}
}