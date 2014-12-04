// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 09.07.2014
//  *******************************************************/


using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LayerAPI.Interfaces;

namespace ForestLayer.Environment {
    internal class _2DEnvironment {
        private readonly IDictionary<Position2D, IAgent> _agentPositions;
        private readonly IDictionary<IAgent, Position2D> _positionsOfAgents;

        private readonly HashSet<Position2D> _freePositions;


        public _2DEnvironment(int maxx, int maxy) {
            _agentPositions = new ConcurrentDictionary<Position2D, IAgent>();
            _freePositions = new HashSet<Position2D>();
            _positionsOfAgents = new ConcurrentDictionary<IAgent, Position2D>();
            for (int x = 0; x < maxx; x++) {
                for (int y = 0; y < maxy; y++) {
                    _freePositions.Add(new Position2D(x, y));
                }
            }
        }


        public void ReleaseMyPosition(IAgent agent) {
            lock (_freePositions) {
                _freePositions.Add(_positionsOfAgents[agent]);
            }
            _positionsOfAgents.Remove(agent);
        }

        public void RandomlyAddAgentsToFreeFields(IEnumerable<IAgent> agents) {
            foreach (IAgent agent in agents) {
                Position2D freePos = _freePositions.First();
                _agentPositions.Add(freePos, agent);
                _freePositions.Remove(freePos);
                _positionsOfAgents.Add(agent, freePos);
            }
        }

        public IEnumerable<IAgent> ExploreRadius(IAgent agent) {
            Position2D agentPosition = _positionsOfAgents[agent];
            List<IAgent> result = new List<IAgent>();

            Position2D posToCheck = new Position2D(agentPosition.X, agentPosition.Y + 1);
            if (_agentPositions.ContainsKey(posToCheck)) result.Add(_agentPositions[posToCheck]);

            posToCheck = new Position2D(agentPosition.X, agentPosition.Y - 1);
            if (_agentPositions.ContainsKey(posToCheck)) result.Add(_agentPositions[posToCheck]);

            posToCheck = new Position2D(agentPosition.X + 1, agentPosition.Y);
            if (_agentPositions.ContainsKey(posToCheck)) result.Add(_agentPositions[posToCheck]);

            posToCheck = new Position2D(agentPosition.X - 1, agentPosition.Y);
            if (_agentPositions.ContainsKey(posToCheck)) result.Add(_agentPositions[posToCheck]);

            posToCheck = new Position2D(agentPosition.X - 1, agentPosition.Y - 1);
            if (_agentPositions.ContainsKey(posToCheck)) result.Add(_agentPositions[posToCheck]);

            posToCheck = new Position2D(agentPosition.X + 1, agentPosition.Y + 1);
            if (_agentPositions.ContainsKey(posToCheck)) result.Add(_agentPositions[posToCheck]);

            posToCheck = new Position2D(agentPosition.X - 1, agentPosition.Y + 1);
            if (_agentPositions.ContainsKey(posToCheck)) result.Add(_agentPositions[posToCheck]);

            posToCheck = new Position2D(agentPosition.X + 1, agentPosition.Y - 1);
            if (_agentPositions.ContainsKey(posToCheck)) result.Add(_agentPositions[posToCheck]);
            return result;
        }
    }
}