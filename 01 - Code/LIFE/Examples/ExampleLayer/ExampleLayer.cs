// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoAPI.Geometries;
using LayerAPI.Interfaces;
using LayerAPI.Interfaces.Visualization;
using MessageWrappers;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace ExampleLayer {
    [Extension(typeof (ISteppedLayer))]
    public class ExampleLayer : ISteppedActiveLayer, IVisualizable {
        private const int agentCount = 10000;
        private List<AgentSmith> _agents;

        #region ISteppedActiveLayer Members

        public bool InitLayer<I>
            (I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            _2DEnvironment _environment = new _2DEnvironment(100, 100);
            _agents = new List<AgentSmith>();
            for (int i = 0; i < agentCount; i++) {
                _agents.Add(new AgentSmith(_environment, unregisterAgentHandle, this));
            }

            //_environment.RandomlyAddAgentsToFreeFields(_agents);

            foreach (AgentSmith agentSmith in _agents) {
                registerAgentHandle.Invoke(this, agentSmith);
            }

            return true;
        }

        public long GetCurrentTick() {

        }

        public void SetCurrentTick(long currentTick) {

        }


        public void Tick() {
            Console.WriteLine("Hello from Tick");
        }

        public void PreTick() {
            Console.WriteLine("Hello from PreTick");
        }

        public void PostTick() {
            Console.WriteLine("Hello from PostTick");
        }

        #endregion

        #region IVisualizable Members

        public List<BasicVisualizationMessage> GetVisData() {
            ConcurrentBag<BasicVisualizationMessage> result = new ConcurrentBag<BasicVisualizationMessage>();
            result.Add(new TerrainDataMessage(100, 0, 100));
            Parallel.ForEach
                (_agents,
                    a => result.Add
                        (new BasicAgent() {
                            Id = a.AgentID.ToString(),
                            Description = "AgentSmith",
                            State = a.Dead ? "Dead" : "Alive",
                            X = a.MyPosition.X,
                            Y = a.MyPosition.Y
                        }));
            return result.ToList();
        }

        public List<BasicVisualizationMessage> GetVisData(IGeometry geometry) {
            return new List<BasicVisualizationMessage>();
        }

        #endregion
    }
}