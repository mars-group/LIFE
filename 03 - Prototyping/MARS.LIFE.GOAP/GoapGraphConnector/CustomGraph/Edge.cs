using System;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.CustomGraph {
    public class Edge : IGoapEdge {
        private readonly IGoapVertex _source;
        private readonly IGoapVertex _target;
        private int _cost;

        public Edge(int cost, IGoapVertex source, IGoapVertex target) {
            _cost = cost;
            _source = source;
            _target = target;
        }

       public IGoapVertex GetSource() {
            return _source;
        }

        public IGoapVertex GetTarget() {
            return _target;
        }

        public int GetCost() {
            return _cost;
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
            if (obj.GetType() != GetType()) return false;
            return Equals((Edge) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((_source != null ? _source.GetHashCode() : 0)*397) ^
                       (_target != null ? _target.GetHashCode() : 0);
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