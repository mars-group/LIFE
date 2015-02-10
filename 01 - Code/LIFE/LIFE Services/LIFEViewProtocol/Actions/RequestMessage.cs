using LIFEViewProtocol.Basics;
using ProtoBuf;

namespace LIFEViewProtocol.Actions {
	[ProtoContract]
	public class RequestMessage : BasicVisualizationMessage {
		/*
		 * 0 - Normal Terrain
		 * 1 - Temperature
		 * 2 - Humidity
		 */
		public string DataType { get; set; }

		public RequestMessage() {
			GetInheritancePath();
		}
	}
}