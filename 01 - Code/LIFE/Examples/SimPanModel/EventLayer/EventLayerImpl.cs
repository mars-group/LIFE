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
        public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public static readonly Dictionary<int, List<int>> PanicTime =
            new Dictionary<int, List<int>> {
                {0, new List<int> {68, 2}},
                {3, new List<int> {25, 4}},
                {5, new List<int> {200, 3}}
            };

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

        public void SetCurrentTick(long currentTick) {
            throw new NotImplementedException();
        }

        #endregion
    }
}