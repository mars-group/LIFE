﻿using System;
using LayerAPI.Layer;
using Mono.Addins;


[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace BugReproductionModel
{
	[Extension(typeof (ISteppedLayer))]
	public class BugReproductionLayer: ISteppedLayer
	{
		public BugReproductionLayer ()
		{
		}

		public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle,
			UnregisterAgent unregisterAgentHandle) {

			for (int i = 0; i < 50; i++) {
				registerAgentHandle.Invoke(this, new SuicideAgent(i, this, unregisterAgentHandle));
			}

			for (int i = 0; i < 25; i++) {
				registerAgentHandle.Invoke(this, new BreedAgent(this, registerAgentHandle, unregisterAgentHandle));
			}
				
			Console.WriteLine ("Layer with equals initialized");

			return true;
		}

	    public long GetCurrentTick() {
	        throw new NotImplementedException();
	    }

	    public void SetCurrentTick(long currentTick) {
	        throw new NotImplementedException();
	    }
	}
}

