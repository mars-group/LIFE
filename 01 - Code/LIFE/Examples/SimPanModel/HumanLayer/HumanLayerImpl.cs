using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayerAPI.Interfaces;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace HumanLayer
{
    [Extension(typeof(ISteppedLayer))]
    public class HumanLayerImpl : ISteppedLayer
    {
        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            return true;
        }

        public long GetCurrentTick() {
            throw new NotImplementedException();
        }
    }
}
