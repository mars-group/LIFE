using System;
using System.Collections.Generic;
using System.Linq;
using GoapBetaCommon.Interfaces;

namespace GoapBetaGraphConnector.SimpleGraph {
    public class Node : IGoapNode, IEquatable<Node> {
        
        private readonly int _heuristic;
        private readonly List<IGoapWorldProperty> _goalValues;
        private readonly List<IGoapWorldProperty> _currValues;
        private List<IGoapWorldProperty> _unsatisfiedGoalValues;
        private List<IGoapWorldProperty> _satisfiedGoalValues;

        public List<IGoapWorldProperty> GetUnsatisfiedGoalValues(){
            return _unsatisfiedGoalValues;
        }

        public int GetHeuristic() {
            return _heuristic;
        }

        public Node(List<IGoapWorldProperty> goalValues, List<IGoapWorldProperty> currentValues, int heuristik) {
            _goalValues = goalValues;
            _currValues = currentValues;
            _heuristic = heuristik;
            CalculateUnsatisfiedAndSatisfiedConditions();
        }

        private void CalculateUnsatisfiedAndSatisfiedConditions() {
            var unsatisfiedGoalValues = new List<IGoapWorldProperty>();
            var satisfiedGoalValues = new List<IGoapWorldProperty>();

            foreach (var goalValue in _goalValues) {
                if (_currValues.Contains(goalValue)) {
                    satisfiedGoalValues.Add(goalValue);
                }else{
                    unsatisfiedGoalValues.Add(goalValue);
                }
            }

            _unsatisfiedGoalValues = unsatisfiedGoalValues;
            _satisfiedGoalValues = satisfiedGoalValues;
        }
        
        public bool HasUnsatisfiedProperties() {
            return _unsatisfiedGoalValues.Count > 0;
        }

        public bool CanBeSatisfiedByStartState(List<IGoapWorldProperty> startState) {
            return _unsatisfiedGoalValues.All(unsatisfiedGoalValue => startState.Contains(unsatisfiedGoalValue));
        }

        public bool Equals(Node other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return (_currValues.Where(x => other._currValues.Contains(x)).Count() == _currValues.Count() && other._currValues.Where(x => _currValues.Contains(x)).Count() == other._currValues.Count()

                && _goalValues.Where(x => other._goalValues.Contains(x)).Count() == _goalValues.Count() && other._goalValues.Where(x => _goalValues.Contains(x)).Count() == other._goalValues.Count()
                && _heuristic == other._heuristic);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
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