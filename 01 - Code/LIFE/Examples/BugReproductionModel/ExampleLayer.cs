using System;
using LayerAPI.Interfaces;
using Mono.Addins;
using System.IO;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace BugReproductionModel
{
	[Extension(typeof (ISteppedLayer))]
	public class ExampleLayer: ISteppedLayer
	{
		public ExampleLayer ()
		{
		}

		public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle,
			UnregisterAgent unregisterAgentHandle) {

			for (int i = 0; i < 500000; i++) {
				registerAgentHandle.Invoke(this, new SuicideAgent(i, this, unregisterAgentHandle));
			}
				
			Console.WriteLine ("Layer with equals initialized");

			return true;
		}

		public long GetCurrentTick() {
			throw new NotImplementedException();
		}
	}
}

