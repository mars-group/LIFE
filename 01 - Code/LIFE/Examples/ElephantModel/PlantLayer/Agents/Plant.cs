using LayerAPI.Interfaces;
using Mono.Addins;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoDimEnvironment;
using System.Windows;

namespace PlantLayer.Agents
{
	public class Plant : IAgent, ISimObject2D
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
        }

        public event System.EventHandler BoundsChanged;
    }
}
