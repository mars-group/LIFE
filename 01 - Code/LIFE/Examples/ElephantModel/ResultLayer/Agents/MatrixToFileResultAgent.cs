using System;
using LayerAPI.Interfaces;
using System.IO;

namespace ResultLayer
{
	public class MatrixToFileResultAgent  : IAgent
	{
		private ElephantLayer _elephantLayer;
		private PlantLayer _plantLayer;
		private WaterLayer _waterLayer;

		public MatrixToFileResultAgent(ElephantLayer elephantLayer, PlantLayer plantLayer, WaterLayer waterlayer){
			_elephantLayer = elephantLayer;
			_plantLayer = plantLayer;
			_waterLayer = waterlayer;
		}

		#region ITickClient implementation

		public void Tick ()
		{

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

