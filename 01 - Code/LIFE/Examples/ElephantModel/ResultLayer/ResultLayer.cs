using LayerAPI.Interfaces;
using Mono.Addins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Size = System.Windows.Size;
using PlantLayer;
using WaterLayer;
using ElephantLayer;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace ResultLayer
{
    public class ResultLayer : ISteppedLayer
    {
		private ElephantLayer _elephantLayer;
		private PlantLayer _plantLayer;
		private WaterLayer _waterLayer;

		public ResultLayer(ElephantLayer elephantLayer, PlantLayer plantLayer, WaterLayer waterlayer){
			_elephantLayer = elephantLayer;
			_plantLayer = plantLayer;
			_waterLayer = waterlayer;
		}

        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            
        }

        public long GetCurrentTick() {
            throw new NotImplementedException();
        }
    }
}
