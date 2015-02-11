using LIFEViewProtocol.Basics;
using ProtoBuf;

namespace LIFEViewProtocol.Actions {
	[ProtoContract]
	public class TerrainInformationRequestMessage : BasicVisualizationMessage {
		[ProtoMember(1)]
		public string DataType { get; private set; }

		protected TerrainInformationRequestMessage() {
			GetInheritancePath();
		}

		public TerrainInformationRequestMessage(string dataType)
		{
			DataType = dataType;
		}
	}
}