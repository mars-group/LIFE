

namespace ExampleLayer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using LayerAPI.Interfaces;

    internal class _2DEnvironment
    {
        private int _maxy;
        private int _maxx;

        private IDictionary<Position2D, AgentSmith> agentPositions;
        private IDictionary<AgentSmith, Position2D> positionsOfAgents;

        private HashSet<Position2D> freePositions; 


        public _2DEnvironment(int maxx, int maxy) {
            _maxx = maxx;
            _maxy = maxy;

            agentPositions = new ConcurrentDictionary<Position2D, AgentSmith>();
            freePositions = new HashSet<Position2D>();
            positionsOfAgents = new ConcurrentDictionary<AgentSmith, Position2D>();
            for (var x = 0; x < maxx; x++) {
                for (var y = 0; y < maxy; y++) {
                    freePositions.Add(new Position2D(x, y));
                }
            }
        }

        public void ReleaseMyPosition(AgentSmith agent) {
            freePositions.Add(positionsOfAgents[agent]);
            positionsOfAgents.Remove(agent);
        }

        public void RandomlyAddAgentsToFreeFields(List<AgentSmith> agents)
        {
            foreach (var agent in agents) {
                var freePos = freePositions.First();
                agentPositions.Add(freePos,agent);
                freePositions.Remove(freePos);
                positionsOfAgents.Add(agent,freePos);
            }
        }

        public List<AgentSmith> ExploreRadius(AgentSmith agent) {
            var agentPosition = positionsOfAgents[agent];
            var result = new List<AgentSmith>();

            var posToCheck = new Position2D(agentPosition.X, agentPosition.Y + 1);
            if (agentPositions.ContainsKey(posToCheck)) {
                result.Add(agentPositions[posToCheck]);    
            }

            posToCheck = new Position2D(agentPosition.X, agentPosition.Y - 1);
            if (agentPositions.ContainsKey(posToCheck))
            {
                result.Add(agentPositions[posToCheck]);
            }

            posToCheck = new Position2D(agentPosition.X + 1, agentPosition.Y);
            if (agentPositions.ContainsKey(posToCheck))
            {
                result.Add(agentPositions[posToCheck]);
            }

            posToCheck = new Position2D(agentPosition.X - 1, agentPosition.Y);
            if (agentPositions.ContainsKey(posToCheck))
            {
                result.Add(agentPositions[posToCheck]);
            }

            posToCheck = new Position2D(agentPosition.X - 1, agentPosition.Y - 1);
            if (agentPositions.ContainsKey(posToCheck))
            {
                result.Add(agentPositions[posToCheck]);
            }

            posToCheck = new Position2D(agentPosition.X + 1, agentPosition.Y + 1);
            if (agentPositions.ContainsKey(posToCheck))
            {
                result.Add(agentPositions[posToCheck]);
            }

            posToCheck = new Position2D(agentPosition.X - 1, agentPosition.Y + 1);
            if (agentPositions.ContainsKey(posToCheck))
            {
                result.Add(agentPositions[posToCheck]);
            }

            posToCheck = new Position2D(agentPosition.X + 1, agentPosition.Y - 1);
            if (agentPositions.ContainsKey(posToCheck))
            {
                result.Add(agentPositions[posToCheck]);
            }
            return result;
        } 


    }
}
