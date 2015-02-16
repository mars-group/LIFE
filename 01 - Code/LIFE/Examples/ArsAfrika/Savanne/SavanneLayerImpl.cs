using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using GisCoordinatesLayer;
using LCConnector.TransportTypes;
using LifeAPI.Agent;
using LifeAPI.Layer;
using LifeAPI.Layer.Visualization;
using LIFEViewProtocol.AgentsAndEvents;
using LIFEViewProtocol.Basics;
using LIFEViewProtocol.Helper;
using LIFEViewProtocol.Terrain;
using Mono.Addins;
using Savanne.Agents;
using SpatialAPI.Environment;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace Savanne {

    [Extension(typeof (ISteppedLayer))]
    public class SavanneLayerImpl : ISteppedActiveLayer, IVisualizable {
        private const string CoordinatesFile = "..\\..\\..\\Examples\\ArsAfrika\\Savanne\\Skukuza_Trees_LatLon.txt";
           

        private const int CountOfAgents = 4000;
        private readonly List<IAgent> _allAgentsOnLayer = new List<IAgent>();

        private readonly IEnvironment _esc =
            new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();

        private readonly TerrainDataMessage _terrainMessage = new TerrainDataMessage(1, 1, 0.0, 1);

        private readonly IGisCoordinatesLayer _coordinatesLayer;
        private long _tick;

        public SavanneLayerImpl(IGisCoordinatesLayer coordinateLayer) {
            _coordinatesLayer = coordinateLayer;
        }

        #region ISteppedActiveLayer Members

        public bool InitLayer (TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {

            List<Tuple<double, double>> coordinates = HelperUtil.ReadKoordinatesFromFile(CoordinatesFile);
            
            Console.WriteLine("Starting agents ...");

            for (int i = 0; i < CountOfAgents; i++) {
                Marula ourAwesomeMarula = new Marula
                    (_esc,
                        500,
                        5,
                        0,
                        1000,
                        Marula.Sex.Male,
                        coordinates[i].Item1,
                        coordinates[i].Item2,
                        _coordinatesLayer);
                _allAgentsOnLayer.Add(ourAwesomeMarula);
                registerAgentHandle.Invoke(this, ourAwesomeMarula);
            }

            Console.WriteLine("Finished agents...");
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

        #region IVisualizable Members

        /// <summary>
        ///     Here the data for the visualisation is constucted in a message format which is pushed to the rabbitMQ.
        /// </summary>
        /// <returns></returns>
        public List<BasicVisualizationMessage> GetVisData() {
            ConcurrentBag<BasicVisualizationMessage> result = new ConcurrentBag<BasicVisualizationMessage> {
                _terrainMessage
            };
            foreach (Marula agent in _allAgentsOnLayer) {
                result.Add
                    (new NonMovingBasicAgent
                        (
                        Definitions.AgentTypes.TreeAgent,
                        new Dictionary<string, string> {{"Stage", "Adult"}},
                        agent.visualisationPosX,
                        0,
                        agent.visualisationPosY,
                        0,
                        agent.ID.ToString(),
                        _tick,
                        1,
                        1,
                        1,
                        "Marula"));
            }
            return result.ToList();
        }

        public List<BasicVisualizationMessage> GetVisData(IGeometry geometry) {
            throw new NotImplementedException();
        }

        #endregion
    }

}