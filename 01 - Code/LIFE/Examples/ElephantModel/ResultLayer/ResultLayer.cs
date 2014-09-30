using LayerAPI.Interfaces;
using Mono.Addins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpQuadTree;
using Size = System.Windows.Size;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace ResultLayer
{
    class ResultLayer : ISteppedLayer
    {
        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            throw new NotImplementedException();
        }

        public long GetCurrentTick() {
            throw new NotImplementedException();
        }
    }
}
