using MessageWrappers.Basics;
using ProtoBuf;

namespace MessageWrappers {
	[ProtoContract]
	public class RemoveMessage : BasicVisualizationMessage {
		[ProtoMember(1)]
		public string Id { get; private set; }

		[ProtoMember(2)]
		public string Message { get; private set; }

		protected RemoveMessage() {
			GetInheritancePath();
		}

		public RemoveMessage(string id, long ticknumber, string message = null) {
			Id = id;
			TickNumber = ticknumber;
			Message = message;
			GetInheritancePath();
		}
	}
}