using System;

namespace GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock {
    internal class GoapWorldStateIsHungry : GoapWorldState, IEquatable<GoapWorldStateIsHungry> {
        public GoapWorldStateIsHungry(bool startValue) : base(startValue) {}

        private const int MaxHunger = 10;
        private const int MinHunger = 0;
        private int _hungerValue = MinHunger;


        public override void CalculateIfWorldStateIsFullfilled() {
            Tick();
            IsStateFulfilled = _hungerValue > 5;
            if (IsStateFulfilled) Console.WriteLine("The Agent is HUNGRY");
        }

        public override void Tick() {
            if (_hungerValue < MaxHunger) _hungerValue += 1;
        }

        public override void PullDependingValues() {}


        public bool Equals(GoapWorldStateIsHungry other) {
            if (ReferenceEquals(this, other)) return true;
            return _hungerValue == other._hungerValue && IsStateFulfilled.Equals(other.IsStateFulfilled);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GoapWorldStateIsHungry) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (_hungerValue*397) ^ IsStateFulfilled.GetHashCode();
            }
        }

        public static bool operator ==(GoapWorldStateIsHungry left, GoapWorldStateIsHungry right) {
            return Equals(left, right);
        }

        public static bool operator !=(GoapWorldStateIsHungry left, GoapWorldStateIsHungry right) {
            return !Equals(left, right);
        }
    }
}