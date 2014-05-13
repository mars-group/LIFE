using System;

namespace GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock {

    internal class GoapWorldStateIsHungry : GoapWorldState, IEquatable<GoapWorldStateIsHungry> {

        internal GoapWorldStateIsHungry(bool startValue) : base(startValue) {
            if (startValue) {
                this._hungerValue = 0;
            }
        }

        private const int MaxHunger = 10;
        private const int MinHunger = 0;
        private int _hungerValue = MaxHunger;


        public override void CalculateIfWorldStateIsFullfilled() {
            Tick();
            IsStateFulfilled = _hungerValue > 5;
            Console.WriteLine(IsStateFulfilled ? "The Agent is HUNGRY" : "The Agent is not hungry");
        }

        public override void Tick() {
            if (_hungerValue < MaxHunger) {
                _hungerValue += 1;
                Console.WriteLine("Hunger is increasing by 1 and now at " + _hungerValue);
            }
        }

        public void ReduceHunger(int minus) {
            if (_hungerValue < minus) _hungerValue = MinHunger;
            else _hungerValue -= minus;
            Console.WriteLine("Hunger is reduced by minus and now at " + _hungerValue);
            this.CalculateIfWorldStateIsFullfilled();
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