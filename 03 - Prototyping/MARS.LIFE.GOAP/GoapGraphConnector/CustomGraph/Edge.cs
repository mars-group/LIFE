using System;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.CustomGraph {
    public class Edge : IGoapEdge, IEquatable<Edge> {
        private readonly IGoapVertex _source;
        private readonly IGoapVertex _target;
        private readonly int _cost;
        private readonly string _name;


        public Edge(int cost, IGoapVertex source, IGoapVertex target, string name = "NotNamedEdge") {
            _cost = cost;
            _source = source;
            _target = target;
            _name = name;
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

        public string Name
        {
            get { return _name; }
        }    

        public override string ToString() {
            return string.Format("Edge: |{0} -> {1}| ", _source, _target);
        }

        public bool Equals(Edge other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_source, other._source) && Equals(_target, other._target) && _cost == other._cost && string.Equals(_name, other._name);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Edge) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (_source != null ? _source.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_target != null ? _target.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ _cost;
                hashCode = (hashCode*397) ^ (_name != null ? _name.GetHashCode() : 0);
                return hashCode;
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