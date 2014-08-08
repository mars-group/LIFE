using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.CustomGraph
{
    class Edge : IGoapEdge, IEquatable<Edge> {
        private IGoapVertex _source;
        private IGoapVertex _target;
        
        public IGoapVertex GetSource() {
            return this._source;
        }

        public IGoapVertex GetTarget() {
            return this._target;
        }

        public override string ToString() {
            return string.Format("Edge: |{0} -> {1}| ", _source, _target);
        }

        public bool Equals(Edge other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_source, other._source) && Equals(_target, other._target);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Edge) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((_source != null ? _source.GetHashCode() : 0)*397) ^ (_target != null ? _target.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Edge left, Edge right) {
            return Equals(left, right);
        }

        public static bool operator !=(Edge left, Edge right) {
            return !Equals(left, right);
        }
    }
}
