using System;
using KNPElevationLayer;
using LCConnector.TransportTypes;
using LifeAPI.Layer;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace KNPTreeLayer
{
    [Extension(typeof(ISteppedLayer))]
    public class TreeLayer : ISteppedLayer
    {
        private long _currentTick;
        private ElevationLayer _elevationLayer;

        public TreeLayer(ElevationLayer elevationLayer) {
            _elevationLayer = elevationLayer;
        }

        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            // TODO: Implement AgentInitializer or something ;-)
            throw new NotImplementedException();
        }

        public long GetCurrentTick() {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick) {
            _currentTick = currentTick;
        }
    }
}
