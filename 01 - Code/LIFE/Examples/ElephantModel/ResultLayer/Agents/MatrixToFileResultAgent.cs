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

		public MatrixToFileResultAgent(ElephantLayerImpl elephantLayer, PlantLayerImpl plantLayer, WaterLayerImpl waterlayer){
			_elephantLayer = elephantLayer;
			_plantLayer = plantLayer;
			_waterLayer = waterlayer;
			_tickCount = 0;
		}

		#region ITickClient implementation

		public void Tick ()
		{
			Console.WriteLine("MatrixWriter reportin' in!");
			// WriteOut Matrix for plant layer
			var plants = _plantLayer.GetAllPlants();

			var stb = new StringBuilder ();

			var x = 0.0;
			plants.ForEach(plant => {
				if(plant.GetBounds().X > x) {
					stb.Append("\r\n" + plant.GetHealth());
					x = plant.GetBounds().X;
				} else {
					stb.Append(plant.GetHealth());
				}
			});

			WriteMatrixToFile(_plantLayer.GetType ().ToString (), stb.ToString (), "Tick-" + _tickCount);
			_tickCount++;
		}

		#endregion

		private void WriteMatrixToFile(string layerName, string matrix, string filename){
			// Write the string to a file.
			System.IO.StreamWriter file = new System.IO.StreamWriter("." + Path.PathSeparator + layerName + "MatrixOutputs" + Path.PathSeparator + filename);
			file.WriteLine(matrix);
			file.Close();
		}
	}
}

