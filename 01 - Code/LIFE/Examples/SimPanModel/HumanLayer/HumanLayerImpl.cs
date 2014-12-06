using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
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
        private const int CountOfHumans = 14;
        private const int CountOfRandomWalker = 6;
        private const int CountOfCalmingHumans = 0;
        private const int CountOfMassFlightLeader = 0;
        private const int CountOfMassFlightAndCalmingLeader = 2;


        public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly CellLayerImpl _cellLayer;
        private readonly ConcurrentDictionary<Guid, Human> _humanList = new ConcurrentDictionary<Guid, Human>();
        private long _currentTick;

        public HumanLayerImpl(CellLayerImpl cellLayer) {
            XmlConfigurator.Configure(new FileInfo("conf.log4net"));
            _cellLayer = cellLayer;
        }

        #region ISteppedLayer Members

        public bool InitLayer<I>
            (I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            for (int i = 1; CountOfHumans >= i; i++) {
                Human humanAgent = new Human(_cellLayer);
                humanAgent.SetTarget(new Point(10, 10));
                _humanList.GetOrAdd(humanAgent.AgentID, humanAgent);
                registerAgentHandle.Invoke(this, humanAgent);
            }

            for (int i = 1; CountOfRandomWalker >= i; i++) {
                Human humanAgent = new Human(_cellLayer);
                _humanList.GetOrAdd(humanAgent.AgentID, humanAgent);
                registerAgentHandle.Invoke(this, humanAgent);
            }

            for (int i = 1; CountOfCalmingHumans >= i; i++) {
                Human humanAgent = new Human(_cellLayer);
                humanAgent.SetCalmingSphere();
                humanAgent.SetTarget(new Point(11, 20));
                _humanList.GetOrAdd(humanAgent.AgentID, humanAgent);
                registerAgentHandle.Invoke(this, humanAgent);
            }

            for (int i = 1; CountOfMassFlightLeader >= i; i++) {
                Human humanAgent = new Human(_cellLayer);
                humanAgent.SetMassFlightSphere();
                humanAgent.SetTarget(new Point(11, 20));
                _humanList.GetOrAdd(humanAgent.AgentID, humanAgent);
                registerAgentHandle.Invoke(this, humanAgent);
            }

            for (int i = 1; CountOfMassFlightAndCalmingLeader >= i; i++) {
                Human humanAgent = new Human(_cellLayer);
                humanAgent.SetMassFlightSphere();
                humanAgent.SetCalmingSphere();
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
        ///     Get reference of agents on the human layer by the given guid list.
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public List<Human> GetHumansById(List<Guid> guidList) {
            List<Human> humanList = _humanList.Values.Where(human => guidList.Contains(human.AgentID)).ToList();
            return humanList;
        }
    }

}