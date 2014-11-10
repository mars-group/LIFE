using System;
using System.Collections.Generic;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.CustomQuickGraph {
    internal class GoapQuickGraphVertex : IEquatable<GoapQuickGraphVertex>, IGoapVertex {

        private readonly List<IGoapWorldstate> _worldStates;
        private readonly string _name;

        public GoapQuickGraphVertex(List<IGoapWorldstate> worldStates, string name = "unnamed") {
            _worldStates = worldStates;
            _name = name;
        }

        public override string ToString() {
            return string.Format("Name: {0}",  _name);
        }

        public bool Equals(GoapQuickGraphVertex other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
           // if (this._worldStates.Count == 0 && other._worldStates.Count == 0) return true;
            return Equals(_worldStates, other._worldStates);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GoapQuickGraphVertex) obj);
        }

        public override int GetHashCode() {
            return (_worldStates != null ? _worldStates.GetHashCode() : 0);
        }

        public int GetHeuristic(IGoapVertex target) {
            throw new NotImplementedException();
        }

        public static bool operator ==(GoapQuickGraphVertex left, GoapQuickGraphVertex right) {
            return Equals(left, right);
        }

        public static bool operator !=(GoapQuickGraphVertex left, GoapQuickGraphVertex right) {
            return !Equals(left, right);
        }
    }
}