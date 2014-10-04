using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.SimpleGraph {
    public class Node : IGoapNode, IEquatable<Node> {
        
        private readonly int _heuristic;
        private readonly List<IGoapWorldProperty> _goalValues;
        private readonly List<IGoapWorldProperty> _currValues;
        private readonly List<IGoapWorldProperty> _unsatisfiedGoalValues;

        public List<IGoapWorldProperty> GetUnsatisfiedGoalValues()
        {
            return _unsatisfiedGoalValues;
        }

        public List<IGoapWorldProperty> GetCurrValues() {
            return _currValues;
        }

        public List<IGoapWorldProperty> GetGoalValues() {
            return _goalValues;
        }

        public int GetHeuristic() {
            return _heuristic;
        }

        public Node(List<IGoapWorldProperty> goalValues, List<IGoapWorldProperty> currentValues, int heuristik) {
            _goalValues = goalValues;
            _currValues = currentValues;
            _heuristic = heuristik;
            _unsatisfiedGoalValues = CalculateUnsatisfiedConditions();
        }
        
        public List<IGoapWorldProperty> CalculateUnsatisfiedConditions() {
            List<IGoapWorldProperty> unsatisfied = new List<IGoapWorldProperty>();
            foreach (var goalValue in _goalValues) {
                if (!_currValues.Contains(goalValue)) unsatisfied.Add(goalValue);
            }
            return unsatisfied;
        }

      
        public List<IGoapWorldProperty> GetSatisfiedGoalValues(){
            List<IGoapWorldProperty> satisfied = new List<IGoapWorldProperty>();
            foreach (var goalValue in _goalValues){
                if (_currValues.Contains(goalValue)) satisfied.Add(goalValue);
            }
            return satisfied;
        }

        public bool HasUnsatisfiedProperties() {
            return _unsatisfiedGoalValues.Count > 0;
        }

       public bool Equals(Node other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _heuristic == other._heuristic && Equals(_goalValues, other._goalValues) && Equals(_currValues, other._currValues) && Equals(_unsatisfiedGoalValues, other._unsatisfiedGoalValues);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Node) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = _heuristic;
                hashCode = (hashCode*397) ^ (_goalValues != null ? _goalValues.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_currValues != null ? _currValues.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_unsatisfiedGoalValues != null ? _unsatisfiedGoalValues.GetHashCode() : 0);
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