using LayerAPI.Interfaces;
using Mono.Addins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoDimEnvironment;
using System.Drawing;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace WaterLayer
{
    public class WaterLayer : ISteppedLayer
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

        public long GetCurrentTick() {
            throw new NotImplementedException();
        }
    }
}
