using System;
using LayerAPI.Interfaces;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace ElephantLayer
{
    public class ElephantLayer : ISteppedLayer
    {
        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            throw new NotImplementedException();
        }

        public long GetCurrentTick() {
            throw new NotImplementedException();
        }
    }
}
