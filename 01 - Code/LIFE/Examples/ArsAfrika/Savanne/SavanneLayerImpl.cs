using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using GeoAPI.Geometries;
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
    internal class SavanneLayerImpl : ISteppedActiveLayer, IVisualizable {
        
        private readonly List<IAgent> _allAgentsOnLayer = new List<IAgent>();
        private const string CoordinatesFile = "C:\\GITVerzeichnis\\LIFE\\01 - Code\\LIFE\\Examples\\ArsAfrika\\Savanne\\Skukuza_Trees_LatLon.txt";
        
        private readonly IEnvironment _esc =
            new EnvironmentServiceComponent.Implementation.EnvironmentServiceComponent();

        private readonly TerrainDataMessage _terrainMessage = new TerrainDataMessage(1, 1, 0.0, 1);

        private long _tick;

        #region ISteppedActiveLayer Members

        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgentHandle) {
            List<Tuple<double, double>> coordinates = HelperUtil.ReadKoordinatesFromFile(CoordinatesFile);
            
            Console.WriteLine("Starting 1 agent ...");

            for (int i = 0; i < 4000; i++) {
                Marula ourAwesomeMarula = new Marula(_esc, 500, 5, 0, 1000, Marula.Sex.Male, coordinates[i].Item1,
                    coordinates[i].Item2);
                _allAgentsOnLayer.Add(ourAwesomeMarula);
                registerAgentHandle.Invoke(this, ourAwesomeMarula);

                //_esc.Add(ourAwesomeMarula.SpacialTreeEntity, new Vector3(coordinates[i].Item1, coordinates[i].Item2));
                // int countOfAgents = _esc.ExploreAll().Count();
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

        public void PostTick() {
            //TODO log stuff: sum of trees
        }

        #endregion

        #region IVisualizable Members

        public List<BasicVisualizationMessage> GetVisData() {
            ConcurrentBag<BasicVisualizationMessage> result = new ConcurrentBag<BasicVisualizationMessage> {
                _terrainMessage
            };
            foreach (Marula agent in _allAgentsOnLayer) {
                result.Add(new NonMovingBasicAgent(
                    Definitions.AgentTypes.TreeAgent,
                    agent.PosX,
                    0,
                    agent.PosY,
                    0,
                    agent.ID.ToString(),
                    _tick,
                    1, 1, 1,
                    new Dictionary<string, string> {{"Stage", "Adult"}},
                    "Marula"));
            }

            return result.ToList();
        }

        //(Definitions.AgentTypes type, double x, double y, double z, float rotation, string id,
        //	long ticknumber, float sizeX, float sizeY, float sizeZ, Dictionary<string, string> attributes, string species,
        //		List<GroupDefinition> groups = null)

        public List<BasicVisualizationMessage> GetVisData(IGeometry geometry) {
            throw new NotImplementedException();
        }

        #endregion
    }
}