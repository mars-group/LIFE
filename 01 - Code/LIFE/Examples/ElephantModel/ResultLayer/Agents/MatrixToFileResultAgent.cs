using System;
using LayerAPI.Interfaces;
using System.IO;
using PlantLayer;
using WaterLayer;
using ElephantLayer;
using System.Text;

namespace ResultLayer
{
	public class MatrixToFileResultAgent  : IAgent
	{
		private ElephantLayerImpl _elephantLayer;
		private PlantLayerImpl _plantLayer;
		private WaterLayerImpl _waterLayer;

		private int _tickCount;

		private string _simrunDateTime;

		public MatrixToFileResultAgent(ElephantLayerImpl elephantLayer, PlantLayerImpl plantLayer, WaterLayerImpl waterlayer){
			_elephantLayer = elephantLayer;
			_plantLayer = plantLayer;
			_waterLayer = waterlayer;
			_tickCount = 0;
			_simrunDateTime = DateTime.Now.ToString("HH:mm:ss tt zzz");
		}

		#region ITickClient implementation

		public void Tick ()
		{
			//Console.WriteLine("MatrixWriter reportin' in!");
			// WriteOut Matrix for plant layer
			var plants = _plantLayer.GetAllPlants();

			var stb = new StringBuilder ();

			var x = 0.0;
			plants.ForEach(plant => {
				if(plant.GetBounds().X > x) {
					stb.Append("\r\n" + plant.GetHealth() + " ");
					x = plant.GetBounds().X;
				} else {
					stb.Append(plant.GetHealth() + " ");
				}
			});

			WriteMatrixToFile ("plantlayer", stb.ToString (), "Tick-" + _tickCount + ".asc");

			var elephants = _elephantLayer.GetAllElephants();

			var stbEle = new StringBuilder ();

			x = 0.0;
			elephants.ForEach(elephant => {
				if(elephant.GetBounds().X > x) {
					stbEle.Append("\r\n" + elephant.Center.X + "/" + elephant.Center.Y + " ");
					x = elephant.GetBounds().X;
				} else {
					stbEle.Append(elephant.Center.X + "/" + elephant.Center.Y + " ");
				}
			});

			WriteMatrixToFile ("elephantLayer", stbEle.ToString (), "Tick-" + _tickCount + ".asc");
			_tickCount++;
		}

		#endregion

		private void WriteMatrixToFile(string layerName, string matrix, string filename)
		{
			// Write the string to a file.
			var dirpath = "." + "/" + layerName + "MatrixOutputsFrom" + "/";
			if (!Directory.Exists (dirpath)) {
				Directory.CreateDirectory (dirpath);
			}
			var filepath = Path.Combine (dirpath, filename);
			File.WriteAllText(filepath, matrix);
		}
	}
}

