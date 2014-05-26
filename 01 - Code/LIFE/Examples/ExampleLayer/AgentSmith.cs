using System;
using LayerAPI.Interfaces;

namespace ExampleLayer {
    using System.Linq;

    internal class AgentSmith : IAgent, IEquatable<AgentSmith> {
        private readonly _2DEnvironment _2DEnvironment;

        private Guid AgentID;

        public AgentSmith(_2DEnvironment _2DEnvironment) {
            this._2DEnvironment = _2DEnvironment;
            this.AgentID = Guid.NewGuid();
            Dead = false;
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
                    Console.WriteLine("No one left :(");
                }
            }
            else {
                Console.WriteLine("I'm dead.");
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