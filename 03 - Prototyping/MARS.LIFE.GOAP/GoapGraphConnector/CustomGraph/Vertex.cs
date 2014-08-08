using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.CustomGraph
{
    class Vertex : IGoapVertex, IEquatable<Vertex> {
        private List<IGoapWorldstate> _worldstate;

        public Vertex(List<IGoapWorldstate> worldstate) {
            _worldstate = worldstate;
        }

        public int GetHeuristic(IGoapVertex target)
        {
            return 1;
        }

        public override string ToString() {
            return string.Format("Vertex: |{0}|", _worldstate);
        }

        public bool Equals(Vertex other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_worldstate, other._worldstate);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Vertex) obj);
        }

        public override int GetHashCode() {
            return (_worldstate != null ? _worldstate.GetHashCode() : 0);
        }

        public static bool operator ==(Vertex left, Vertex right) {
            return Equals(left, right);
        }

        public static bool operator !=(Vertex left, Vertex right) {
            return !Equals(left, right);
        }
    }
}
