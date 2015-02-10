using System;
using System.Collections.Generic;
using LCConnector.TransportTypes;
using LifeAPI.Agent;
using LifeAPI.Layer;
using Mono.Addins;
using Savanne.Agents;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Environment;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace Savanne {
    [Extension(typeof (ISteppedLayer))]
    internal class SavanneLayerImpl : ISteppedActiveLayer {
        private readonly List<IAgent> _allAgentsOnLayer = new List<IAgent>();
        private long _tick;
        public IEnvironment Esc = new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();

        #region ISteppedActiveLayer Members

        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgentHandle) {
            Console.WriteLine("Starting 1 agent ...");

            for (int i = 1; i < 10; i++) {
                Marula ourAwesomeMarula = new Marula(i*10, i*10, Esc);
                _allAgentsOnLayer.Add(ourAwesomeMarula);
                registerAgentHandle.Invoke(this, ourAwesomeMarula);
                Esc.Add(ourAwesomeMarula.SpacialTreeEntity, Vector3.Zero);
                IEnumerable<ISpatialEntity> test = Esc.ExploreAll();
            }
            
            Console.WriteLine("Finished agent...");
            return true;
        }

        public long GetCurrentTick() {
            return _tick;
        }

        public void SetCurrentTick(long currentTick) {
            _tick = currentTick;
        }

        public void Tick() {}

        public void PreTick() {}

        public void PostTick() {}

        #endregion
    }
}