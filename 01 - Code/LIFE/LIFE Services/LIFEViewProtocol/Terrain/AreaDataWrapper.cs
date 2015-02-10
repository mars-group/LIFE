using MessageWrappers.Basics;
using MessageWrappers.Terrain;
using ProtoBuf;

namespace MessageWrappers {
	[ProtoContract]
	public class AreaDataWrapper : BasicVisualizationMessage {
		[ProtoMember(1)]
		public TiledTerrainData[,] HeightData { get; set; }
	}
}