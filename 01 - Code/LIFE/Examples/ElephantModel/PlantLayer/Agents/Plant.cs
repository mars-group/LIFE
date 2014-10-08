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
        private double _health;

		private Rect _bounds;

		public Plant(float x, float y, Size size)
        {
			_bounds.X = x;
			_bounds.Y = y;
			_bounds.Size = size;
			_health = 100;
        }

        public double GetHealth()
        {
			return _health;
        }

        public void SubHealth(double x) 
        {
			_health -= x;
        }

        public void Tick()
        {
			if (_health > 0.0) {
				// regenerate a tiny bit
				//_health += _health * 0.0001;
			}
        }


        public Rect Bounds
        {
			get { return _bounds; }
			set { _bounds = value; }
        }

        public event System.EventHandler BoundsChanged;
    }
}
