using System;
using LayerAPI.Interfaces;

namespace ExampleLayer {
    using System.Linq;

    internal class AgentSmith : IAgent, IEquatable<AgentSmith> {
        private readonly _2DEnvironment _2DEnvironment;
        private readonly UnregisterAgent _unregisterAgentHandle;
        private readonly ExampleLayer _thislayer;
        public Position2D MyPosition { get; private set; }

        public Guid AgentID { get; private set; }

        public AgentSmith(_2DEnvironment _2DEnvironment, UnregisterAgent unregisterAgentHandle, ExampleLayer thislayer) {
            this._2DEnvironment = _2DEnvironment;
            _unregisterAgentHandle = unregisterAgentHandle;
            _thislayer = thislayer;
            AgentID = Guid.NewGuid();
            Dead = false;
            MyPosition = _2DEnvironment.SetAgentPosition(this);
        }

        public void Tick() {
            if (!Dead) {
                var enemies = _2DEnvironment.ExploreRadius(this);
                var enemy = enemies.FirstOrDefault();
                if (enemy != null && !enemy.Dead) {
                    enemy.Kill();
                    Console.WriteLine("Killed someone!");
                }
                else {
                    //Console.WriteLine("No one left :(");
                }
            }
            else {
                //Console.WriteLine("I'm dead.");
                _unregisterAgentHandle(_thislayer, this);
            }
            //Console.WriteLine("Tell me, Mr. Anderson, what good is a phone call when you are unable to speak?");
        }

        public void Kill() {
            if (!Dead) {
                Dead = true;
                _2DEnvironment.ReleaseMyPosition(this);
            }
        }

        public bool Dead { get; set; }

        public bool Equals(AgentSmith other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.AgentID.Equals(other.AgentID);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AgentSmith)obj);
        }

        public override int GetHashCode() {
            return this.AgentID.GetHashCode();
        }

        public static bool operator ==(AgentSmith left, AgentSmith right) {
            return Equals(left, right);
        }

        public static bool operator !=(AgentSmith left, AgentSmith right) {
            return !Equals(left, right);
        }
    }
}