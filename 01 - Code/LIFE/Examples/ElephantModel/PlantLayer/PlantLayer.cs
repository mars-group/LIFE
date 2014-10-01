using LayerAPI.Interfaces;
using Mono.Addins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlantLayer.Agents;
using TwoDimEnvironment;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace PlantLayer
{
    class PlantLayer : ISteppedLayer
    {
		private ITwoDimEnvironment<Plant> quadTree;
        public PlantLayer()
        {

        }
        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgentHandle)
        {
			var x = new TwoDimEnvironmentUseCase<Plant>();
			var a = new Plant () {
				Bounds = new System.Windows.Rect (){ }
			};
            return true;
        }

        public long GetCurrentTick() {
            throw new NotImplementedException();
        }

        void stomp (int x, int y, double force)
        {

        }

        List<TPlant> getAllPlants()
        {
            return null;
        }
    }
}
