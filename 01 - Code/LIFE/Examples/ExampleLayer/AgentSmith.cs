
using System;
using System.Threading;
using LifeAPI.Agent;
using LifeAPI.Layer;

namespace ExampleLayer
{
    using System.Linq;

    internal class AgentSmith : IAgent
    {
        private readonly _2DEnvironment _2DEnvironment;
        private readonly UnregisterAgent _unregisterAgentHandle;
        private readonly ExampleLayer _thislayer;
        private readonly object _lock = new object();
        private int _tickcounter;
        public Guid AgentID { get; private set; }
        private readonly Random _random;

        public AgentSmith(_2DEnvironment _2DEnvironment, UnregisterAgent unregisterAgentHandle, ExampleLayer thislayer)
        {
            _random = new Random((int)DateTime.Now.Ticks);
            this._2DEnvironment = _2DEnvironment;
            _unregisterAgentHandle = unregisterAgentHandle;
            _thislayer = thislayer;
            AgentID = Guid.NewGuid();
            Dead = false;
            _tickcounter = 0;
            _2DEnvironment.SetAgentPosition(this);
        }

        public void Tick()
        {
            var enemies = _2DEnvironment.ExploreRadius(this);
            var agentSmiths = enemies.Where(agent => !agent.Dead).ToArray();
            AgentSmith enemey = null;
            if (agentSmiths.Length > 0) enemey = agentSmiths[_random.Next(0, agentSmiths.Count())];
            lock (_lock)
            {
                if (Dead) return;
                if (enemey != null)
                {
                    enemey.TryKill();
                }
                else
                {
                    _2DEnvironment.SetAgentPosition(this);
                }
            }

        }

        private void TryKill()
        {
            if (!Monitor.TryEnter(_lock, 100)) return;
            if (Dead) return;
            Dead = true;
            _unregisterAgentHandle(_thislayer, this);
            _2DEnvironment.ReleaseMyPosition(this);
            Monitor.Exit(_lock);
        }

        public bool Dead { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is AgentSmith)) return false;
            return (obj as AgentSmith).AgentID == AgentID;
        }

        public override int GetHashCode()
        {
            return this.AgentID.GetHashCode();
        }

        public static bool operator ==(AgentSmith left, AgentSmith right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AgentSmith left, AgentSmith right)
        {
            return !Equals(left, right);
        }

        public Guid ID { get; set; }
    }
}