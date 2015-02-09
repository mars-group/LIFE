using ProtoBuf;

namespace MessageWrappers {
	[ProtoContract]
	[ProtoInclude(100, typeof (BasicAgent))]
	[ProtoInclude(101, typeof (ClientCameraViewMessage))]
	[ProtoInclude(102, typeof (RequestMessage))]
	[ProtoInclude(103, typeof (TerrainDataMessage))]
	[ProtoInclude(104, typeof (RegisterMessage))]
	[ProtoInclude(105, typeof (InitializationMessage))]
	[ProtoInclude(106, typeof (GroupDefinition))]
	[ProtoInclude(107, typeof (CommitMessage))]
	[ProtoInclude(108, typeof (TiledTerrainData))]
	[ProtoInclude(109, typeof (Polygon))]
	[ProtoInclude(110, typeof (DataSetMessage))]
	[ProtoInclude(111, typeof (EnvironmentEvent))]
	[ProtoInclude(112, typeof (RemoveMessage))]
	//[ProtoInclude(100, typeof(AreaDataWrapper))]
	public abstract class BasicVisualizationMessage {
		/*
		 * Supported types:
		 * - RequestMessage
		 * - BasicAgent
		 * - AreaDescriptor
		 * - AreaDataWrapper
		 * - VegetationAgent
		 * - MovingBasicAgent
		 * - TerrainDataMessage
		 */

		[ProtoMember(1)]
		public string MessageType { get; set; }

		[ProtoMember(2)]
		public long TickNumber { get; set; }

		protected BasicVisualizationMessage() {
			GetInheritancePath();
		}

		internal void GetInheritancePath() {
			MessageType = "";
			var baseType = GetType();
			while (baseType != null && !MessageType.Contains("BasicVisualizationMessage")) {
				MessageType = baseType + "/" + MessageType;
				baseType = baseType.BaseType;
			}
		}
	}
}