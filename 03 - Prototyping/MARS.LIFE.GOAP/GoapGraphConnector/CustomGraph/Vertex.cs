using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.CustomGraph {
    public class Vertex : IGoapVertex, IEquatable<Vertex> {
        private readonly List<IGoapWorldstate> _worldstate;
        private readonly string _name;
        // for test----------
        private readonly int _heuristic;

        public Vertex(List<IGoapWorldstate> worldstate) {
            _worldstate = worldstate;
        }

        
        public Vertex(List<IGoapWorldstate> worldstate, int heuristic, string name) {
            _worldstate = worldstate;
            _heuristic = heuristic;
            _name = name;
        }

       

        // for test--------------

        public int GetHeuristic(IGoapVertex target) {
            return _heuristic;
        }

        public override string ToString() {
            string states = _worldstate.Aggregate("", (current, state) => current + " " + state.ToString());

            return string.Format("<Knoten {0} {1}>", _name, states);
        }

        public bool Equals(Vertex other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_worldstate, other._worldstate);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
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