using GOAPLayer.Agents;
using LayerAPI.Interfaces;
using Mono.Addins;


[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace GOAPLayer
{
    [Extension(typeof(ISteppedLayer))]
    public class GoapLayerImpl : ISteppedLayer {
        private const string NamespaceOfModelDefinition = "GOAPModelDefinition";
        private const int GoapWorkdandplayCount = 1;
        
        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            var wap = new WorkAndPlay("AgentConfig1", NamespaceOfModelDefinition);
            registerAgentHandle.Invoke(this, wap);

            return true;
        }

        public long GetCurrentTick() {
            throw new System.NotImplementedException();
        }
    }
}
