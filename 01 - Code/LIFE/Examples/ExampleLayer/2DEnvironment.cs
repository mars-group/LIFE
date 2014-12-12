

using System;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using NetTopologySuite.Geometries;

namespace ExampleLayer
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    internal class _2DEnvironment
    {
        private readonly IDictionary<Point, AgentSmith> _posToAgent;
        private readonly IDictionary<AgentSmith, Point> _agentToPos;

        private readonly HashSet<Point> _freePositions;
        private readonly Random _random;

        public _2DEnvironment(int maxx, int maxy)
        {
            _posToAgent = new ConcurrentDictionary<Point, AgentSmith>();
            _agentToPos = new ConcurrentDictionary<AgentSmith, Point>();
            _freePositions = new HashSet<Point>();

            _random = new Random((int)DateTime.Now.Ticks);

            for (var x = 0; x < maxx; x++)
            {
                for (var y = 0; y < maxy; y++)
                {
                    _freePositions.Add(new Point(x, y));
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ReleaseMyPosition(AgentSmith agent)
        {
            if (!_agentToPos.ContainsKey(agent)) return;
            var pos = _agentToPos[agent];
            _agentToPos.Remove(agent);
            _posToAgent.Remove(pos);
            _freePositions.Add(pos);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetAgentPosition(AgentSmith agent)
        {
            ReleaseMyPosition(agent);
            var freePos = _freePositions.ElementAt(_random.Next(0, _freePositions.Count - 1));
            _freePositions.Remove(freePos);
            _posToAgent.Add(freePos, agent);
            _agentToPos.Add(agent, freePos);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal Point GetPosition(AgentSmith agent)
        {
            return _agentToPos[agent];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<AgentSmith> ExploreRadius(AgentSmith agent)
        {
            Point agentPosition;
            if (!_agentToPos.TryGetValue(agent, out agentPosition)) return new AgentSmith[0];

            var result = new List<AgentSmith>();
            AgentSmith outAgent;

            var posToCheck = new Point(agentPosition.X, agentPosition.Y + 1);
            if (_posToAgent.TryGetValue(posToCheck, out outAgent))
            {
                result.Add(outAgent);
            }

            posToCheck = new Point(agentPosition.X, agentPosition.Y - 1);
            if (_posToAgent.TryGetValue(posToCheck, out outAgent))
            {
                result.Add(outAgent);
            }

            posToCheck = new Point(agentPosition.X + 1, agentPosition.Y);
            if (_posToAgent.TryGetValue(posToCheck, out outAgent))
            {
                result.Add(outAgent);
            }

            posToCheck = new Point(agentPosition.X - 1, agentPosition.Y);
            if (_posToAgent.TryGetValue(posToCheck, out outAgent))
            {
                result.Add(outAgent);
            }

            posToCheck = new Point(agentPosition.X - 1, agentPosition.Y - 1);
            if (_posToAgent.TryGetValue(posToCheck, out outAgent))
            {
                result.Add(outAgent);
            }

            posToCheck = new Point(agentPosition.X + 1, agentPosition.Y + 1);
            if (_posToAgent.TryGetValue(posToCheck, out outAgent))
            {
                result.Add(outAgent);
            }

            posToCheck = new Point(agentPosition.X - 1, agentPosition.Y + 1);
            if (_posToAgent.TryGetValue(posToCheck, out outAgent))
            {
                result.Add(outAgent);
            }

            posToCheck = new Point(agentPosition.X + 1, agentPosition.Y - 1);
            if (_posToAgent.TryGetValue(posToCheck, out outAgent))
            {
                result.Add(outAgent);
            }
            return result;
        }
    }
}
