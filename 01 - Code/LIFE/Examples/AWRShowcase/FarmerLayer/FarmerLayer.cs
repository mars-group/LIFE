using System.Collections.Generic;
using AWRShowcase.FarmerLayer.Agents;
using LayerAPI.Interfaces;
using Mono.Addins;


[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace AWRShowcase.FarmerLayer
{
    [Extension(typeof(ISteppedLayer))]
    public class FarmerLayer : ISteppedLayer {
        private const int FarmerCount = 100;
        private readonly ForestLayer _forestLayer;
        private readonly List<Farmer> _farmers;

        public FarmerLayer(ForestLayer forestLayer) {
            _forestLayer = forestLayer;
            _farmers = new List<Farmer>();
        }

        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            for (int i = 0; i < FarmerCount; i++) {
                _farmers.Add(new Farmer(_forestLayer));
            }
            foreach (var farmer in _farmers) {
                registerAgentHandle.Invoke(this, farmer);
            }
            return true;
        }

        public long GetCurrentTick() {
            throw new System.NotImplementedException();
        }
    }
}
