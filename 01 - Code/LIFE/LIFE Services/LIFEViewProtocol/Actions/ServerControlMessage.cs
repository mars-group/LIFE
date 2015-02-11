﻿using LIFEViewProtocol.Basics;
using LIFEViewProtocol.Helper;
using ProtoBuf;

namespace LIFEViewProtocol.Actions {
	[ProtoContract]
	public class ServerControlMessage : BasicVisualizationMessage {
		[ProtoMember(1)]
		public Definitions.ServerActions Action { get; private set; }

		[ProtoMember(2)]
		public string Client { get; private set; }

		protected ServerControlMessage() {
			GetInheritancePath();
		}

		public ServerControlMessage(Definitions.ServerActions action, string client) {
			Action = action;
			Client = client;
			GetInheritancePath();
		}
	}
}