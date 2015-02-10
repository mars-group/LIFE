using MessageWrappers.Basics;
using ProtoBuf;

namespace MessageWrappers.Terrain {
	/// <summary>
	///     Data of one terrain tile
	/// </summary>
	[ProtoContract]
	public class TiledTerrainData : BasicVisualizationMessage {
		/// <summary>
		///     Heights of the terrain.
		/// </summary>
		[ProtoMember(1)]
		public FloatArray2D Heights { get; set; }

		/// <summary>
		///     Contains information about the textures. This may be left empty.
		/// </summary>
		[ProtoMember(2)]
		public FloatArray2D AlphaMap { get; set; }

		/// <summary>
		///     Size of the submitted tiles with a maximum of 2049 (better 1025). Size must be (2^n) + 1 (eg. 513 or 1025)
		/// </summary>
		[ProtoMember(3)]
		public int Tilesize { get; set; }

		[ProtoMember(4)]
		public double MapHeight { get; set; }

		[ProtoMember(5)]
		public int XPosition { get; private set; }

		[ProtoMember(6)]
		public int YPosition { get; private set; }

		[ProtoMember(7)]
		public double WestCoordinate { get; private set; }

		[ProtoMember(8)]
		public double SouthCoordinate { get; private set; }

		[ProtoMember(9)]
		public double EastCoordinate { get; private set; }

		[ProtoMember(10)]
		public double NorthCoordinate { get; private set; }

		[ProtoMember(11)]
		public float Level { get; private set; }


		/// <summary>
		///     Constructor for use with ProtoBuf
		/// </summary>
		protected TiledTerrainData() {
			GetInheritancePath();
		}

		/// <summary>
		///     Constructor containing height- and alphamap.
		/// </summary>
		public TiledTerrainData(FloatArray2D heights, FloatArray2D alphaMap, int tilesize, int mapheight, int xpos, int ypos,
			float level = 0) {
			Heights = heights;
			AlphaMap = alphaMap;
			Tilesize = tilesize;
			MapHeight = mapheight;
			XPosition = xpos;
			YPosition = ypos;
			Level = level;
			GetInheritancePath();
		}

		/// <summary>
		///     Constructor for tiles without alphamap.
		/// </summary>
		public TiledTerrainData(FloatArray2D heights, int tilesize, double mapheight, int xpos, int ypos, float level = 0) {
			Heights = heights;
			Tilesize = tilesize;
			MapHeight = mapheight;
			XPosition = xpos;
			YPosition = ypos;
			Level = level;
			GetInheritancePath();
		}

		/// <summary>
		///     Constructor for simple flat tiles.
		/// </summary>
		public TiledTerrainData(int tilesize, int mapheight, int xpos, int ypos, float level = 0) {
			var map = new float[tilesize, tilesize];
			for (int x = 0; x < tilesize; x++) {
				for (int y = 0; y < tilesize; y++) {
					map[x, y] = 0;
				}
			}
			Heights = new FloatArray2D(map);
			Tilesize = tilesize;
			MapHeight = mapheight;
			XPosition = xpos;
			YPosition = ypos;
			Level = level;
			GetInheritancePath();
		}

		public TiledTerrainData(FloatArray2D heights, int tilesize, double mapHeight, int xPosition, int yPosition,
			double westCoordinate, double southCoordinate, double eastCoordinate, double northCoordinate, float level = 0) {
			GetInheritancePath();
			Heights = heights;
			Tilesize = tilesize;
			MapHeight = mapHeight;
			XPosition = xPosition;
			YPosition = yPosition;
			WestCoordinate = westCoordinate;
			SouthCoordinate = southCoordinate;
			EastCoordinate = eastCoordinate;
			NorthCoordinate = northCoordinate;
			Level = level;
		}

		public TiledTerrainData(FloatArray2D heights, FloatArray2D alphaMap, int tilesize, double mapHeight, int xPosition,
			int yPosition, double westCoordinate, double southCoordinate, double eastCoordinate, double northCoordinate,
			float level) {
			Heights = heights;
			AlphaMap = alphaMap;
			Tilesize = tilesize;
			MapHeight = mapHeight;
			XPosition = xPosition;
			YPosition = yPosition;
			WestCoordinate = westCoordinate;
			SouthCoordinate = southCoordinate;
			EastCoordinate = eastCoordinate;
			NorthCoordinate = northCoordinate;
			Level = level;
			GetInheritancePath();
		}
	}
}