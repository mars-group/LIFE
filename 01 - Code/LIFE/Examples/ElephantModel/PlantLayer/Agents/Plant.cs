using LayerAPI.Interfaces;
using Mono.Addins;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoDimEnvironment;
using System.Windows;
using CSharpQuadTree;

namespace PlantLayer.Agents
{
	public class Plant : IAgent, IQuadObject
    {
        private int Health;

		private Rect _bounds;

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


        public Rect Bounds
        {
			get { return _bounds; }
			set { _bounds = value; }
        }

        public event System.EventHandler BoundsChanged;
    }
}
