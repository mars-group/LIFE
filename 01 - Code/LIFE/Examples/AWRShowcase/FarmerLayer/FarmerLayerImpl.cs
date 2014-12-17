using System.Collections.Generic;
using AWRShowcase.FarmerLayer.Agents;
using ForestLayer;
using LayerAPI.Interfaces;
using Mono.Addins;


[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace FarmerLayer
{
    [Extension(typeof(ISteppedLayer))]
    public class FarmerLayerImpl : ISteppedLayer {
        private const int FarmerCount = 100;
        private readonly ForestLayerImpl _forestLayer;
        private readonly List<Farmer> _farmers;

        public FarmerLayerImpl(ForestLayerImpl forestLayer)
        {
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
