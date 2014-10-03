using LayerAPI.Interfaces;
using Mono.Addins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Size = System.Windows.Size;
using ElephantLayer;
using PlantLayer;
using WaterLayer;


[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace ResultLayer
{
	[Extension(typeof (ISteppedLayer))]
    public class ResultLayer : ISteppedLayer
    {
		private ElephantLayerImpl _elephantLayer;
		private PlantLayerImpl _plantLayer;
		private WaterLayerImpl _waterLayer;

		public ResultLayer(ElephantLayerImpl elephantLayer, PlantLayerImpl plantLayer, WaterLayerImpl waterlayer){
			_elephantLayer = elephantLayer;
			_plantLayer = plantLayer;
			_waterLayer = waterlayer;
		}

        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
			var m = new MatrixToFileResultAgent (_elephantLayer, _plantLayer, _waterLayer);
			registerAgentHandle.Invoke (this, m);
			return true;
        }

        public long GetCurrentTick() {
            throw new NotImplementedException();
        }
    }
}
