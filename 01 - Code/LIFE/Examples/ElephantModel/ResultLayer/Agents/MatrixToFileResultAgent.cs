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

		public MatrixToFileResultAgent(ElephantLayerImpl elephantLayer, PlantLayerImpl plantLayer, WaterLayerImpl waterlayer){
			_elephantLayer = elephantLayer;
			_plantLayer = plantLayer;
			_waterLayer = waterlayer;
		}

		#region ITickClient implementation

		public void Tick ()
		{
			// WriteOut Matrix for plant layer
			var plants = _plantLayer.GetAllPlants();

			var stb = new StringBuilder ();

			plants.ForEach(plant => {
				stb.Append(plant.GetHealth);
			});
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

