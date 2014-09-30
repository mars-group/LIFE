using LayerAPI.Interfaces;
using Mono.Addins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpQuadTree;
using Size = System.Windows.Size;
using PlantLayer.Agents;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace PlantLayer
{
    class PlantLayer : ISteppedLayer
    {
        private QuadTree<Plant> quadTree;
        public PlantLayer()
        {

        }
        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgentHandle)
        {
            var x = new QuadTree<Plant>(new Size(5, 5), 2);
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
