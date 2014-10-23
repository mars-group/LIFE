

using System.Runtime.CompilerServices;
using System.Security.Permissions;

namespace ExampleLayer
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    internal class _2DEnvironment
    {        
        private readonly IDictionary<Position2D, AgentSmith> _agentPositions;
        private readonly IDictionary<AgentSmith, Position2D> _positionsOfAgents;

        private readonly HashSet<Position2D> _freePositions; 


        public _2DEnvironment(int maxx, int maxy) {
            _agentPositions = new ConcurrentDictionary<Position2D, AgentSmith>();
            _freePositions = new HashSet<Position2D>();
            _positionsOfAgents = new ConcurrentDictionary<AgentSmith, Position2D>();
            for (var x = 0; x < maxx; x++) {
                for (var y = 0; y < maxy; y++) {
                    _freePositions.Add(new Position2D(x, y));
                }
            }
        }


        public void ReleaseMyPosition(AgentSmith agent) {
            lock (_freePositions)
            {
                _freePositions.Add(_positionsOfAgents[agent]);
            }
            _positionsOfAgents.Remove(agent);
        }

        public void RandomlyAddAgentsToFreeFields(IEnumerable<AgentSmith> agents)
        {
            foreach (var agent in agents) {
                var freePos = _freePositions.First();
                _agentPositions.Add(freePos,agent);
                _freePositions.Remove(freePos);
                _positionsOfAgents.Add(agent,freePos);
            }
        }

        public IEnumerable<AgentSmith> ExploreRadius(AgentSmith agent) {
            if (agent.Dead) {
                return new List<AgentSmith>(); 
            }
            var agentPosition = _positionsOfAgents[agent];
            var result = new List<AgentSmith>();

            var posToCheck = new Position2D(agentPosition.X, agentPosition.Y + 1);
            if (_agentPositions.ContainsKey(posToCheck)) {
                result.Add(_agentPositions[posToCheck]);    
            }

            posToCheck = new Position2D(agentPosition.X, agentPosition.Y - 1);
            if (_agentPositions.ContainsKey(posToCheck))
            {
                result.Add(_agentPositions[posToCheck]);
            }

            posToCheck = new Position2D(agentPosition.X + 1, agentPosition.Y);
            if (_agentPositions.ContainsKey(posToCheck))
            {
                result.Add(_agentPositions[posToCheck]);
            }

            posToCheck = new Position2D(agentPosition.X - 1, agentPosition.Y);
            if (_agentPositions.ContainsKey(posToCheck))
            {
                result.Add(_agentPositions[posToCheck]);
            }

            posToCheck = new Position2D(agentPosition.X - 1, agentPosition.Y - 1);
            if (_agentPositions.ContainsKey(posToCheck))
            {
                result.Add(_agentPositions[posToCheck]);
            }

            posToCheck = new Position2D(agentPosition.X + 1, agentPosition.Y + 1);
            if (_agentPositions.ContainsKey(posToCheck))
            {
                result.Add(_agentPositions[posToCheck]);
            }

            posToCheck = new Position2D(agentPosition.X - 1, agentPosition.Y + 1);
            if (_agentPositions.ContainsKey(posToCheck))
            {
                result.Add(_agentPositions[posToCheck]);
            }

            posToCheck = new Position2D(agentPosition.X + 1, agentPosition.Y - 1);
            if (_agentPositions.ContainsKey(posToCheck))
            {
                result.Add(_agentPositions[posToCheck]);
            }
            return result;
        } 


    }
}
