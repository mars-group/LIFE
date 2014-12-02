using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CellLayer;
using HumanLayer.Agents;
using LayerAPI.Interfaces;
using log4net;
using log4net.Config;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace HumanLayer {

    [Extension(typeof (ISteppedLayer))]
    public class HumanLayerImpl : ISteppedLayer {

        private long _currentTick;
        private const int CountOfHumans = 20;
        public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);      
        private readonly CellLayerImpl _cellLayer;
        private ConcurrentDictionary<Guid, Human> _humanList = new ConcurrentDictionary<Guid, Human>();
        
        public HumanLayerImpl(CellLayerImpl cellLayer) {
            XmlConfigurator.Configure(new FileInfo("conf.log4net"));
            _cellLayer = cellLayer;
        }

        #region ISteppedLayer Members

        public bool InitLayer<I>
            (I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            for (int i = 0; CountOfHumans >= i; i++) {
                Human humanAgent = new Human(_cellLayer);
                _humanList.GetOrAdd(humanAgent.AgentID, humanAgent);
                registerAgentHandle.Invoke(this, humanAgent);
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

        /// <summary>
        /// Get reference of agents on the human layer by the given guid list.
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public List<Human> GetHumansById(List<Guid> guidList) {
            List<Human> humanList =  _humanList.Values.Where(human => guidList.Contains(human.AgentID)).ToList();
            return humanList;
        } 
    }

}