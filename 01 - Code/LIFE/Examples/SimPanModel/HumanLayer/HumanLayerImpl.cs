using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CellLayer;
using HumanLayer.Agents;
using log4net;
using log4net.Config;
using LCConnector.TransportTypes;
using LifeAPI.Layer;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace HumanLayer {

    [Extension(typeof (ISteppedLayer))]
    public class HumanLayerImpl : ISteppedLayer {
   
        private const int GoapReactiveAgents = 13;
        private const int GoapDeliberativeAgents = 5;
        private const int GoapReflectiveAgents = 2;

        public const int CalmingRadius = 2;
        public const int MassFlightRadius = 2;
        public const int Strength = 5;
        public const int ResistanceToPressure = 10;
        
        public const int LowestBoundOfFear = 1;
        public const int HighestBoundOfFear = 100;
        public const int UpperBoundOfReactiveBehaviourArea = 84;
        public const int UpperBoundOfDeliberativeBehaviourArea = 49;
        public const int UpperBoundOfReflectiveBehaviourArea = 19;
        

        public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly CellLayerImpl _cellLayer;
        private readonly ConcurrentDictionary<Guid, Human> _humanList = new ConcurrentDictionary<Guid, Human>();
        private long _currentTick;

        public const string NamespaceOfModelDefinition = "SimPanInGoapModelDefinition";
        public const string ReactiveConfig = "ReactiveConfig";
        public const string DeliberativeConfig = "DeliberativeConfig";
        public const string ReflectiveConfig = "ReflectiveConfig";

        public HumanLayerImpl(CellLayerImpl cellLayer) {
            XmlConfigurator.Configure(new FileInfo("conf.log4net"));
            _cellLayer = cellLayer;
        }

        #region ISteppedLayer Members

        public bool InitLayer
            (TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {

           for (int i = 1; GoapReactiveAgents >= i; i++){
                Human humanAgent = new Human( _cellLayer, CellLayerImpl.BehaviourType.Reactive);
                _humanList.GetOrAdd(humanAgent.AgentID, humanAgent);
                registerAgentHandle.Invoke(this, humanAgent);
            }

             for (int i = 1; GoapDeliberativeAgents >= i; i++){
                Human humanAgent = new Human( _cellLayer,  CellLayerImpl.BehaviourType.Deliberative);
                _humanList.GetOrAdd(humanAgent.AgentID, humanAgent);
                registerAgentHandle.Invoke(this, humanAgent);
            }

             for (int i = 1; GoapReflectiveAgents >= i; i++){
                Human humanAgent = new Human( _cellLayer,  CellLayerImpl.BehaviourType.Reflective);
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