using ProtoBuf;

namespace MessageWrappers {
	[ProtoContract]
	public class CommitMessage : BasicVisualizationMessage {
		public CommitMessage(long ticknumber) {
			TickNumber = ticknumber;
			GetInheritancePath();
		}

		protected CommitMessage() {
			GetInheritancePath();
		}
	}
}