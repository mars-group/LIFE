using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using HumanLayer.Agents;

namespace SimPanInGoapModelDefinition{


    public abstract class AbstactSimPanGoapAction : AbstractGoapAction
    {
        public AbstactSimPanGoapAction(Human human, List<WorldstateSymbol> preconditionWorldstates, List<WorldstateSymbol> effectWorldstates) :
            base(preconditionWorldstates, effectWorldstates) {}

        public override void Execute() {
            throw new NotImplementedException();
        }
    }
}
