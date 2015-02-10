using LIFEViewProtocol.Basics;
using ProtoBuf;

namespace LIFEViewProtocol.Terrain {
	[ProtoContract]
	public class AreaDataWrapper : BasicVisualizationMessage {
		[ProtoMember(1)]
		public TiledTerrainData[,] HeightData { get; set; }
	}
}