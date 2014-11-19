using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;

namespace SimPanInGoapModelDefinition.Actions {

    public class SuccessiveApproximation : AbstractGoapAction {
        public SuccessiveApproximation
            (List<WorldstateSymbol> preconditionWorldstates, List<WorldstateSymbol> effectWorldstates) :
                base(preconditionWorldstates, effectWorldstates) {}

        public override bool ValidateContextPreconditions() {
            throw new NotImplementedException();
        }

        public override bool ExecuteContextEffects() {
            throw new NotImplementedException();
        }

        public override int GetExecutionCosts() {
            throw new NotImplementedException();
        }
        
        public override void Execute() {
            throw new NotImplementedException();
        }
    }

}