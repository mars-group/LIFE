using ProtoBuf;

namespace MessageWrappers {
	[ProtoContract]
	public class TileArray2D {
		[ProtoMember(1)]
		private TiledTerrainData[] Fields { get; set; }

		[ProtoMember(2)]
		private int Rows { get; set; }

		protected TileArray2D() {}

		/// <summary>
		///     Convert the two-dimensional array to a one-dimensional array so it can be serialized with ProtoBuf
		/// </summary>
		/// <param name="source"></param>
		public TileArray2D(TiledTerrainData[,] source) {
			Rows = source.GetLength(0);
			var y = source.GetLength(1);
			Fields = new TiledTerrainData[Rows*y];
			for (int i = 0; i < Rows; i++) {
				for (int j = 0; j < y; j++) {
					Fields[i*y + j] = source[i, j];
				}
			}
		}

		/// <summary>
		///     Convert the one-dimensional array back to a two-dimensional
		/// </summary>
		/// <returns></returns>
		public TiledTerrainData[,] Get2DFloats() {
			var y = Fields.Length/Rows;
			var array = new TiledTerrainData[Rows, y];
			for (var i = 0; i < Rows; i++) {
				for (var j = 0; j < y; j++) {
					array[i, j] = Fields[i*y + j];
				}
			}
			return array;
		}
	}
}