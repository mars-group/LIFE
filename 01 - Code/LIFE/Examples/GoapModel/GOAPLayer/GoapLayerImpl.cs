using System.Reflection;
using GOAPLayer.Agents;
using log4net;
using LCConnector.TransportTypes;
using LifeAPI.Layer;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace GOAPLayer {

    [Extension(typeof (ISteppedLayer))]
    public class GoapLayerImpl : ISteppedLayer {
        private const string NamespaceOfModelDefinition = "GOAPModelDefinition";
        private const int GoapWorkdAndPlayAgentCount = 50;

        public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private long _currentTick;

        #region ISteppedLayer Members

        public bool InitLayer
            (TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            for (int i = 1; i <= GoapWorkdAndPlayAgentCount; i++) {
                WorkAndPlay wap = new WorkAndPlay("AgentConfig1", NamespaceOfModelDefinition);
                registerAgentHandle.Invoke(this, wap);
            }

            return true;
        }

        public long GetCurrentTick() {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick) {
            _currentTick = currentTick;
        }

        #endregion
    }

}