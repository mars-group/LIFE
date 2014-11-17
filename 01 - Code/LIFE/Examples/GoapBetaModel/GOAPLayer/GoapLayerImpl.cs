using System;
using System.Reflection;
using GOAPLayer.Agents;
using LayerAPI.Interfaces;
using log4net;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace GOAPLayer {

    [Extension(typeof (ISteppedLayer))]
    public class GoapLayerImpl : ISteppedLayer {
        private const string NamespaceOfModelDefinition = "GOAPBetaModelDefinition";
        private const int GoapWorkdAndPlayAgentCount = 1000;

        public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region ISteppedLayer Members

        public bool InitLayer<I>
            (I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {

            for (int i = 1; i <= GoapWorkdAndPlayAgentCount; i++) {
                WorkAndPlay wap = new WorkAndPlay("AgentConfig1", NamespaceOfModelDefinition);
                registerAgentHandle.Invoke(this, wap);
            }
            
            return true;
        }

        public long GetCurrentTick() {
            throw new NotImplementedException();
        }

        #endregion
    }

}