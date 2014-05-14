using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoapComponent.GoapKnowledgeProcessingComponent;
using GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock;

namespace GoapComponent.GoapActionsStock
{
    class GoapActionBuyIce :GoapAction
    {

        public GoapActionBuyIce(Goap goap)
            : base(goap)
        {


            var preSunIsShining = new GoapWorldStateSunIsShining(true);
            var preGotIce = new GoapWorldStateGotIce(false);

            this.Preconditions.Add(preSunIsShining);
            this.Preconditions.Add(preGotIce);

            var postGotIce = new GoapWorldStateGotIce(true);

            this.Postconditions.Add(postGotIce);
        }
    }

    
}
