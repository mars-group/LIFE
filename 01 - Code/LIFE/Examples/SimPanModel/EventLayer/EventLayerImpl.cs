using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CellLayer;
using EventLayer.Agents;
using LayerAPI.Interfaces;
using log4net;
using log4net.Config;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace EventLayer {
    [Extension(typeof (ISteppedLayer))]
    public class EventLayerImpl : ISteppedLayer {

        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        internal static readonly Dictionary<int, int> PanicTime = new Dictionary<int, int> {{0, 68}, {3, 5}, {5, 200}};
        private readonly CellLayerImpl _cellLayer;

        public EventLayerImpl(CellLayerImpl cellLayer) {
            XmlConfigurator.Configure(new FileInfo("conf.log4net"));
            _cellLayer = cellLayer;
        }

        #region ISteppedLayer Members

        public bool InitLayer<I>
            (I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            Event eventAgent = new Event(_cellLayer);
            registerAgentHandle.Invoke(this, eventAgent);
            return true;
        }

        public long GetCurrentTick() {
            throw new NotImplementedException();
        }

        #endregion
    }
}