using System;
using System.Collections.Generic;
using System.Linq;
using GoapBetaCommon.Interfaces;

namespace GoapBetaCommon.Implementation {
    /// <summary>
    ///     node class for use in  graph service
    ///     connect the existing and expected symbols of the worldstate
    /// </summary>
    public class Node : IGoapNode, IEquatable<Node> {
        private readonly int _heuristic;
        private readonly List<IGoapWorldProperty> _goalValues;
        private readonly List<IGoapWorldProperty> _currValues;
        private List<IGoapWorldProperty> _unsatisfiedGoalValues;

        public Node(List<IGoapWorldProperty> goalValues, List<IGoapWorldProperty> currentValues, int heuristik) {
            _goalValues = goalValues;
            _currValues = currentValues;
            _heuristic = heuristik;
            CalculateUnsatisfiedAndSatisfiedConditions();
        }

        #region IEquatable<Node> Members

        public bool Equals(Node other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return (_currValues.Count(x => other._currValues.Contains(x)) == _currValues.Count()
                    && other._currValues.Count(x => _currValues.Contains(x)) == other._currValues.Count()
                    && _goalValues.Count(x => other._goalValues.Contains(x)) == _goalValues.Count()
                    && other._goalValues.Count(x => _goalValues.Contains(x)) == other._goalValues.Count()
                    && _heuristic == other._heuristic);
        }

        #endregion

        #region IGoapNode Members

        public List<IGoapWorldProperty> GetUnsatisfiedGoalValues() {
            return _unsatisfiedGoalValues;
        }

        public int GetHeuristic() {
            return _heuristic;
        }

        /// <summary>
        ///     get the count of unsatisfied symbols
        /// </summary>
        /// <returns></returns>
        public bool HasUnsatisfiedProperties() {
            return _unsatisfiedGoalValues.Count > 0;
        }

        /// <summary>
        ///     check if the given worldstate would satisfy all the unsatisfied symbols
        /// </summary>
        /// <param name="startState"></param>
        /// <returns></returns>
        public bool CanBeSatisfiedByStartState(List<IGoapWorldProperty> startState) {
            return _unsatisfiedGoalValues.All(unsatisfiedGoalValue => startState.Contains(unsatisfiedGoalValue));
        }

        #endregion

        /// <summary>
        ///     compare the expected and existing symbols of the worldstate
        /// </summary>
        private void CalculateUnsatisfiedAndSatisfiedConditions() {
            List<IGoapWorldProperty> unsatisfiedGoalValues = new List<IGoapWorldProperty>();
            List<IGoapWorldProperty> satisfiedGoalValues = new List<IGoapWorldProperty>();

            foreach (IGoapWorldProperty goalValue in _goalValues) {
                if (_currValues.Contains(goalValue)) satisfiedGoalValues.Add(goalValue);
                else unsatisfiedGoalValues.Add(goalValue);
            }
            _unsatisfiedGoalValues = unsatisfiedGoalValues;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Node) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (_currValues != null ? _currValues.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_goalValues != null ? _goalValues.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ _heuristic;
                return hashCode;
            }
        }

        public static bool operator ==(Node left, Node right) {
            return Equals(left, right);
        }

        public static bool operator !=(Node left, Node right) {
            return !Equals(left, right);
        }
    }
}