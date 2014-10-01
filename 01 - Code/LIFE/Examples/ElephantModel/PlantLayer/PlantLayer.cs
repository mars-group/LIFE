using LayerAPI.Interfaces;
using Mono.Addins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlantLayer.Agents;
using TwoDimEnvironment;
using System.Windows;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace PlantLayer
{
    class PlantLayer : ISteppedLayer
    {
		private ITwoDimEnvironment<Plant> environment;
		private List<Plant> _plants;

        public PlantLayer()
        {

        }

        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
			environment = new TwoDimEnvironmentUseCase<Plant>();
			_plants = new List<Plant>();
			// create a 100x100 playboard and add plants everywhere but in the middle
			for (int x = 0; x < 100; x++) {
				for (int y = 0; y < 100; y++) {
					if((x < 48 && y < 48) || (x > 52 && y > 52)){
						var p = new Plant (x, y, new Size (1.0, 1.0));
						registerAgentHandle.Invoke (this, p);
						_plants.Add (p);
						environment.Add(p);
					}
				}
			}

            return true;
        }

        public long GetCurrentTick() {
            throw new NotImplementedException();
        }

        public void Stomp (double x, double y, double force)
        {
			// check if something is there
			var p = environment.Find(new Rect(x,y,1.0,1.0));
			// for everything found, stomp upon it and save it back to environment
			foreach (var plant in p) {
				plant.SubHealth(force);
				environment.Update(plant);
			}
        }

        List<TPlant> getAllPlants()
        {
			var allPlants = environment.GetAll ();
			var result = new List<TPlant> ();
			allPlants.ForEach (p => result.Add(new TPlant(p)));
			return result;
        }
    }
}
