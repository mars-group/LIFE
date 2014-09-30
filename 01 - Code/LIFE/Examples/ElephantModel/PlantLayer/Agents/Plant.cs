using CSharpQuadTree;
using LayerAPI.Interfaces;
using Mono.Addins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantLayer.Agents
{
    public class Plant : IAgent, IQuadObject
    {
        private int Health;
        public Plant()
        {
            Health = 100;
        }
        public int getHealth()
        {
            return Health;
        }
        public void Tick()
        {

        }
    }
}
