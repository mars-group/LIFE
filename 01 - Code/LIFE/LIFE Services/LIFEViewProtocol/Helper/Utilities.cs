using System;
using System.Diagnostics;

namespace MessageWrappers {
	public static class Utilities {
		public static void GenerateMapFromData(float[,] map, double height, out TiledTerrainData[,] ttd,
			out TerrainDataMessage tdm) {
			TiledTerrainData[,] tiled;
			TerrainDataMessage dataMessage;
			GenerateMapFromData(map, 1025, height, out tiled, out dataMessage);
			ttd = tiled;
			tdm = dataMessage;
			// Trace.WriteLine("X: "+dataMessage.TileCountX+" Y: "+dataMessage.TileCountY);
		}

		public static void GenerateMapFromData(float[,] map, double tilesize, double height, out TiledTerrainData[,] ttd,
			out TerrainDataMessage tdm) {
			Trace.WriteLine("map[" + map.GetLength(0) + "," + map.GetLength(1) + "] xCount: " + map.GetLength(0)/tilesize +
			                " yCount: " + map.GetLength(1)/tilesize);
			int xCount = Convert.ToInt32(Math.Ceiling((map.GetLength(0)/tilesize)));
			int yCount = Convert.ToInt32(Math.Ceiling((map.GetLength(1)/tilesize)));
			ttd = new TiledTerrainData[xCount, yCount];
			for (int xTilePos = 0; xTilePos < xCount; xTilePos++) {
				for (int yTilePos = 0; yTilePos < yCount; yTilePos++) {
					float[,] tile = new float[(int) tilesize, (int) tilesize];
					Array.Clear(tile, 0, tile.Length);
					var xMax = Math.Min(tilesize, map.GetLength(0) - ((xTilePos)*tilesize));
					var yMax = Math.Min(tilesize, map.GetLength(1) - ((yTilePos)*tilesize));
					for (int x = 0; x < xMax; x++) {
						for (int y = 0; y < yMax; y++) {
							tile[x, y] = map[(int) ((xTilePos*tilesize) + x), (int) ((yTilePos*tilesize) + y)];
						}
					}
					ttd[xTilePos, yTilePos] = new TiledTerrainData(new FloatArray2D(tile), Convert.ToInt32(tilesize), height, xTilePos,
						yTilePos);
				}
			}
			tdm = new TerrainDataMessage(xCount, yCount, height, (int) tilesize);
		}

		public static void GenerateMapFromData(float[,] map, double tilesize, double height, double cellsize, int epsg,
			double west, double south, double east, double north, out TiledTerrainData[,] ttd, out TerrainDataMessage tdm) {
			Trace.WriteLine("map[" + map.GetLength(0) + "," + map.GetLength(1) + "] xCount: " + map.GetLength(0)/tilesize +
			                " yCount: " + map.GetLength(1)/tilesize);
			int xCount = Convert.ToInt32(Math.Ceiling((map.GetLength(0)/tilesize)));
			int yCount = Convert.ToInt32(Math.Ceiling((map.GetLength(1)/tilesize)));
			ttd = new TiledTerrainData[xCount, yCount];
			for (int xTilePos = 0; xTilePos < xCount; xTilePos++) {
				for (int yTilePos = 0; yTilePos < yCount; yTilePos++) {
					float[,] tile = new float[(int) tilesize, (int) tilesize];
					Array.Clear(tile, 0, tile.Length);
					var xMax = Math.Min(tilesize, map.GetLength(0) - ((xTilePos)*tilesize));
					var yMax = Math.Min(tilesize, map.GetLength(1) - ((yTilePos)*tilesize));
					for (int x = 0; x < xMax; x++) {
						for (int y = 0; y < yMax; y++) {
							tile[x, y] = map[(int) ((xTilePos*tilesize) + x), (int) ((yTilePos*tilesize) + y)];
						}
					}
					var dist = tilesize*cellsize;
					var xOffset = 0.0d;
					var yOffset = 0.0d;
					if (xTilePos > 0) xOffset = cellsize;
					if (yTilePos > 0) yOffset = cellsize;
					var w = CheckLongitude(west + (xTilePos*dist) + xOffset);
					var e = CheckLongitude(w + dist);
					var s = CheckLatitude(south + (yTilePos*dist) + yOffset);
					var n = CheckLatitude(s + dist);
					ttd[xTilePos, yTilePos] = new TiledTerrainData(new FloatArray2D(tile), Convert.ToInt32(tilesize), height, xTilePos,
						yTilePos, w, s, e, n);
				}
			}
			tdm = new TerrainDataMessage(xCount, yCount, height, (int) tilesize, epsg, west, south, east, north, cellsize);
		}

		public static double CheckLatitude(double lat) {
			double res = lat;
			if (lat > 90) res = lat - (lat - 90)*2;
			else if (lat < -90) res = lat - (lat + 90)*2;

			return res;
		}

		public static double CheckLongitude(double lon) {
			double res = lon;
			if (lon > 180) res = lon - 360;
			else if (lon < -180) res = lon + 360;

			return res;
		}

		public static void GenerateSimpleMap(int xSize, int ySize, int height, out TiledTerrainData[,] ttd,
			out TerrainDataMessage tdm) {
			GenerateSimpleMap(xSize, ySize, height, 1025, out ttd, out tdm);
		}

		public static void GenerateSimpleMap(int xSize, int ySize, int height, double tilesize, out TiledTerrainData[,] ttd,
			out TerrainDataMessage tdm) {
			int xCount = Convert.ToInt32(Math.Ceiling((xSize/tilesize)));
			int yCount = Convert.ToInt32(Math.Ceiling((ySize/tilesize)));
			ttd = new TiledTerrainData[xCount, yCount];
			for (int xTilePos = 0; xTilePos < xCount; xTilePos++) {
				for (int yTilePos = 0; yTilePos < yCount; yTilePos++) {
					/*float[,] tile = new float[(int)tilesize, (int)tilesize];
					Array.Clear(tile, 0, tile.Length);*/
					ttd[xTilePos, yTilePos] = new TiledTerrainData(null, Convert.ToInt32(tilesize), height, xTilePos, yTilePos);
				}
			}
			tdm = new TerrainDataMessage(xCount, yCount, height, (int) tilesize);
		}
	}
}