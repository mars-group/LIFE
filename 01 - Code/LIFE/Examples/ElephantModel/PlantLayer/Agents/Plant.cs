using CSharpQuadTree;
using LayerAPI.Interfaces;
using Mono.Addins;
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
        public int GetHealth()
        {
            return Health;
        }

        public void SubHealth(int x) 
        {
            
        }
        public void Tick()
        {

        }


        public System.Windows.Rect Bounds
        {
            get { throw new System.NotImplementedException(); }
        }

        public event System.EventHandler BoundsChanged;
    }
}
