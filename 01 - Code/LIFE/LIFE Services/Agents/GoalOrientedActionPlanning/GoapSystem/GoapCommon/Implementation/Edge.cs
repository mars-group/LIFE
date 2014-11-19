using System;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;

namespace GoapCommon.Implementation {

    /// <summary>
    ///     used in graph for plan creation
    /// </summary>
    public class Edge : IGoapEdge, IEquatable<Edge> {
        private readonly IGoapNode _source;
        private readonly IGoapNode _target;
        private readonly AbstractGoapAction _action;

        public Edge(AbstractGoapAction action, IGoapNode source, IGoapNode target) {
            _action = action;
            _source = source;
            _target = target;
        }

        #region IEquatable<Edge> Members

        public bool Equals(Edge other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return Equals(_target, other._target) && Equals(_action, other._action) && Equals(_source, other._source);
        }

        #endregion

        #region IGoapEdge Members

        public IGoapNode GetSource() {
            return _source;
        }

        public IGoapNode GetTarget() {
            return _target;
        }

        public AbstractGoapAction GetAction() {
            return _action;
        }

        public int GetCost() {
            return _action.GetExecutionCosts();
        }

        #endregion

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != GetType()) {
                return false;
            }
            return Equals((Edge) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (_target != null ? _target.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_action != null ? _action.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_source != null ? _source.GetHashCode() : 0);
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