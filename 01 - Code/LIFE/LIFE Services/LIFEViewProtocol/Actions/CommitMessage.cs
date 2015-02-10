using LIFEViewProtocol.Basics;
using ProtoBuf;

namespace LIFEViewProtocol.Actions {
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