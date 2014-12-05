using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CellLayer;
using EventLayer.Agents;
using HumanLayer;
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

        /// <summary>
        ///     Defines the tick number, panik cell number and range of chaos cells of a panic event.
        /// </summary>
        public static readonly Dictionary<int, List<int>> PanicTime =
            new Dictionary<int, List<int>> {
                {0, new List<int> {68, 2}},
                //{3, new List<int> {25, 4}},
                //{5, new List<int> {200, 3}}
            };

        private readonly CellLayerImpl _cellLayer;
        private readonly HumanLayerImpl _humanLayer;
        private long _currentTick;

        public EventLayerImpl(CellLayerImpl cellLayer, HumanLayerImpl humanLayer) {
            XmlConfigurator.Configure(new FileInfo("conf.log4net"));
            _cellLayer = cellLayer;
            _humanLayer = humanLayer;
        }

        #region ISteppedLayer Members

        public bool InitLayer<I>
            (I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            Event eventAgent = new Event(_cellLayer, _humanLayer);
            registerAgentHandle.Invoke(this, eventAgent);
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