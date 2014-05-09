using System;

namespace GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock {
    internal class GoapWorldStateSunIsShining : GoapWorldState, IEquatable<GoapWorldStateSunIsShining> {
        public GoapWorldStateSunIsShining(bool startValue) : base(startValue) {}

        public override void CalculateIfWorldStateIsFullfilled() {
            throw new NotImplementedException();
        }

        public override void PullDependingValues() {
            throw new NotImplementedException();
        }

        public override void Tick() {}

        public bool Equals(GoapWorldStateSunIsShining other) {
            if (ReferenceEquals(this, other)) return true;
            return IsStateFulfilled.Equals(other.IsStateFulfilled);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GoapWorldStateSunIsShining) obj);
        }

        public override int GetHashCode() {
            return IsStateFulfilled.GetHashCode();
        }

        public static bool operator ==(GoapWorldStateSunIsShining left, GoapWorldStateSunIsShining right) {
            return Equals(left, right);
        }

        public static bool operator !=(GoapWorldStateSunIsShining left, GoapWorldStateSunIsShining right) {
            return !Equals(left, right);
        }
    }
}