using System;
using LCConnector.TransportTypes;
using LifeAPI.Layer;
using SpatialAPI.Environment;

namespace KNPEnvironmentLayer
{
    public class KNPEnvironmentLayer : IKNPEnvironmentLayer
    {
        private IEnvironment _esc;
        private long _currentTick;

        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            _esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();
            return true;
        }


        public long GetCurrentTick() {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick) {
            _currentTick = currentTick;
        }
    }
}
