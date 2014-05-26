using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock
{
    class GoapWorldStateGotIce : GoapWorldState
    {

        public GoapWorldStateGotIce(bool startValue) : base(startValue) {}


        public override void CalculateIfWorldStateIsFullfilled() {
            
        }

        public void SetOwningIce() {
            this.IsStateFulfilled = true;
            Console.WriteLine("The Agent got Ice");
        }

        public void SetNoIce() {
            this.IsStateFulfilled = false;
            Console.WriteLine("The Agent lost Ice");
        }

        public override void PullDependingValues() {
           
        }

        public override void Tick() {
            
        }
    }

    
}
