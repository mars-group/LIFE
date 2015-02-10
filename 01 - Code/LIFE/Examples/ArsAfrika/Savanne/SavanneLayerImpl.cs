using System;
using System.Collections.Generic;
using LCConnector.TransportTypes;
using LifeAPI.Agent;
using LifeAPI.Layer;
using Mono.Addins;
using Savanne.Agents;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Environment;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace Savanne {
    [Extension(typeof (ISteppedLayer))]
    internal class SavanneLayerImpl : ISteppedActiveLayer {
        private readonly List<IAgent> _allAgentsOnLayer = new List<IAgent>();

        private readonly IEnvironment _esc =
            new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();

        private long _tick;

        #region ISteppedActiveLayer Members

        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgentHandle) {
            Console.WriteLine("Starting 1 agent ...");

            for (int i = 1; i < 4000; i++) {
                Marula ourAwesomeMarula = new Marula(1, 1, _esc, 500, 5, 0, 1000, Marula.Sex.Male);
                _allAgentsOnLayer.Add(ourAwesomeMarula);
                registerAgentHandle.Invoke(this, ourAwesomeMarula);

                //ESC is generally more interesting for moving agents. Maybe it is not needed for this model
                _esc.Add(ourAwesomeMarula.SpacialTreeEntity, new Vector3(i*10, i*10));
            }
            List<Marula> test = null;
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

        public void PostTick() {
            //TODO log stuff: sum of trees
        }

        #endregion
    }
}