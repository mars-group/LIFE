using ProtoBuf;

namespace MessageWrappers {
	/// <summary>
	///     Message containing basic informations about the tiled terrain
	/// </summary>
	[ProtoContract]
	public class TerrainDataMessage : BasicVisualizationMessage {
		/// <summary>
		///     Overall width of the terrain (x-axis)
		/// </summary>
		[ProtoMember(1)]
		public int TileCountX { get; private set; }

		/// <summary>
		///     Overall depth of the terrain (y-axis)
		/// </summary>
		[ProtoMember(2)]
		public int TileCountY { get; private set; }

		/// <summary>
		///     Overall height of the terrain (z-axis)
		/// </summary>
		[ProtoMember(3)]
		public double Height { get; private set; }

		/// <summary>
		///     Size of the tiles
		/// </summary>
		[ProtoMember(4)]
		public int TileSize { get; private set; }

		/// <summary>
		///     If real world coordinates are used, this field contains the coordinate reference system
		///     See also: http://de.wikipedia.org/wiki/European_Petroleum_Survey_Group_Geodesy#EPSG-Codes
		///     Most likely you will use 4326 (OSM Database, GPS) or 3857 (Google Maps, OSM etc.)
		/// </summary>
		[ProtoMember(6)]
		public int EPSGCode { get; private set; }

		[ProtoMember(7)]
		public double WestCoordinate { get; private set; }

		[ProtoMember(8)]
		public double SouthCoordinate { get; private set; }

		[ProtoMember(9)]
		public double EastCoordinate { get; private set; }

		[ProtoMember(10)]
		public double NorthCoordinate { get; private set; }

		[ProtoMember(11)]
		public double CellSize { get; private set; }

		/// <summary>
		///     Basic constructor, needed for ProtoBuf
		/// </summary>
		protected TerrainDataMessage() {
			GetInheritancePath();
		}

		/// <summary>
		///     Constructor for a terrain using coordinates
		/// </summary>
		/// <param name="tileCountX"></param>
		/// <param name="tileCountY"></param>
		/// <param name="height"></param>
		/// <param name="tileSize"></param>
		/// <param name="epsgCode"></param>
		/// <param name="westCoordinate"></param>
		/// <param name="southCoordinate"></param>
		/// <param name="eastCoordinate"></param>
		/// <param name="northCoordinate"></param>
		/// <param name="cellSize"></param>
		public TerrainDataMessage(int tileCountX, int tileCountY, double height, int tileSize, int epsgCode,
			double westCoordinate, double southCoordinate, double eastCoordinate, double northCoordinate, double cellSize) {
			TileCountX = tileCountX;
			TileCountY = tileCountY;
			Height = height;
			TileSize = tileSize;
			EPSGCode = epsgCode;
			WestCoordinate = westCoordinate;
			SouthCoordinate = southCoordinate;
			EastCoordinate = eastCoordinate;
			NorthCoordinate = northCoordinate;
			CellSize = cellSize;
		}

		/// <summary>
		///     Constructor
		/// </summary>
		/// <param name="tileCountX">Number of tiles in width (x-axis)</param>
		/// <param name="tileCountY">Number of tiles in depth (y-axis)</param>
		/// <param name="height">Overall height (z-axis)</param>
		/// <param name="tileSize">Size of a single tile</param>
		public TerrainDataMessage(int tileCountX, int tileCountY, double height, int tileSize) {
			TileCountX = tileCountX;
			TileCountY = tileCountY;
			Height = height;
			TileSize = tileSize;
			GetInheritancePath();
		}

		public override string ToString() {
			return "Sizes X: " + TileCountX + " Z: " + TileCountY + " Y: " + Height + " Heightmap: " + TileSize;
		}
	}
}