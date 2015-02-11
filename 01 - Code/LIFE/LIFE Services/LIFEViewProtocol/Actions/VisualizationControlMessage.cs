﻿using LIFEViewProtocol.Basics;
using LIFEViewProtocol.Helper;
using ProtoBuf;

namespace LIFEViewProtocol.Actions {
	[ProtoContract]
	public class VisualizationControlMessage : BasicVisualizationMessage {
		[ProtoMember(1)]
		public Definitions.VisualizationActions Action { get; private set; }

		protected VisualizationControlMessage() {
			GetInheritancePath();
		}

		public VisualizationControlMessage(Definitions.VisualizationActions action) {
			Action = action;
			GetInheritancePath();
		}
	}
}