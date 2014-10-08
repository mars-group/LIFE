using LayerAPI.Interfaces;
using Mono.Addins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoDimEnvironment;
using System.Windows;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace WaterLayer
{
	[Extension(typeof (ISteppedLayer))]
    public class WaterLayerImpl : ISteppedLayer
    {
		private ITwoDimEnvironment<Waterhole> environment;

		private List<Waterhole> _waterholes;

        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
			environment = new TwoDimEnvironmentUseCase<Waterhole>();
			_waterholes = new List<Waterhole>();

			var p = new Waterhole(48, 52, new Size (5.0, 5.0));
			registerAgentHandle.Invoke (this, p);
			_waterholes.Add (p);
			environment.Add(p);

			return true;
        }

		public List<TWaterhole> getAllWaterholes()
		{
			var allWaterholes = environment.GetAll ();
			var result = new List<TWaterhole> ();
			allWaterholes.ForEach (w => result.Add(new TWaterhole(w)));
			return result;
		}

		public List<TWaterhole> Probe(double x, double y, double distance){
			var holes = environment.Find(new Rect(x,y,distance,distance));
			var result = new List<TWaterhole> ();
			holes.ForEach (h => result.Add (new TWaterhole(h)));
			return result;
		}


        public long GetCurrentTick() {
            throw new NotImplementedException();
        }
    }
}
