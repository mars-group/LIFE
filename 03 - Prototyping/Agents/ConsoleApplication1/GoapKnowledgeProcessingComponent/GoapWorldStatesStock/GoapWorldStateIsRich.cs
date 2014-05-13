using System;

namespace GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock
{
    class GoapWorldStateIsRich : GoapWorldState
    {
        public GoapWorldStateIsRich(bool startValue) : base(startValue) {}

        private const int PovertyLine = 10;
        private int _balance = 0;


        public override void CalculateIfWorldStateIsFullfilled() {
            IsStateFulfilled = _balance > PovertyLine;
        }

        public void EarnOneDollar() {
            _balance += 1;
            Console.WriteLine("Agent is getting rich. Current money amount is " + _balance);
        }

        public override void PullDependingValues() {
            throw new NotImplementedException();
        }

        public override void Tick() {
            throw new NotImplementedException();
        }
    }
}
