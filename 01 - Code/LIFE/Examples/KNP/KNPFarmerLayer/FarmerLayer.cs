using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Hik.Communication.ScsServices.Service;
using KNPElevationLayer;
using KNPEnvironmentLayer;
using KNPFarmerLayer.Agents;
using LCConnector.TransportTypes;
using LifeAPI.Layer;
using Mono.Addins;
using TreeLayer;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace KNPFarmerLayer
{
    [Extension(typeof(ISteppedLayer))]
    public class FarmerLayer : ScsService, IKNPFarmerLayer
    {
        private readonly IKnpElevationLayer _elevationLayer;
        private readonly IKNPEnvironmentLayer _environmentLayer;
        private readonly IKnpTreeLayer _treeLayer;
        private long _currentTick;

        private double MinX = 31.331;
        private double MinY = -25.292;
        private double MaxX = 31.985;
        private double MaxY = -24.997;

        public FarmerLayer(IKnpElevationLayer elevationLayer, IKNPEnvironmentLayer environmentLayer, IKnpTreeLayer treeLayer) {
            _elevationLayer = elevationLayer;
            _environmentLayer = environmentLayer;
            _treeLayer = treeLayer;
        }

        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            foreach (var agentInitConfig in layerInitData.AgentInitConfigs) {
                if (agentInitConfig.AgentName != "Farmer") continue;
                var agentBag = new ConcurrentBag<Farmer>();

                var config = agentInitConfig;
                var placementError = false;
                Parallel.For(0, agentInitConfig.RealAgentCount, i => {
                    var farmer = new Farmer(
                        _elevationLayer,
                        _environmentLayer,
                        _treeLayer,
                        config.RealAgentIds[i],
                        GetRandomDouble(MinX, MaxX),
                        GetRandomDouble(MinY, MaxY));

                    // add to ESC 
                    if (!_environmentLayer.Add(farmer.SpatialEntity, farmer.SpatialEntity.Shape.Position)) placementError = true;

                    agentBag.Add(farmer);
                    registerAgentHandle(this, farmer);
                });

                if (placementError) return false;
            }

            return true;
        }

        public long GetCurrentTick() {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick) {
            _currentTick = currentTick;
        }

        private double GetRandomDouble(double minimum, double maximum)
        {
            var random = new Random(54897439);
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
