using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;

namespace SimPanInGoapModelDefinition.Actions {

    public class AimlessWandering : AbstractGoapAction {
        public AimlessWandering
            (List<WorldstateSymbol> preconditionWorldstates, List<WorldstateSymbol> effectWorldstates) :
                base(preconditionWorldstates, effectWorldstates) {}

        public override bool ValidateContextPreconditions() {
            return true;
        }

        public override bool ExecuteContextEffects() {
            throw new NotImplementedException();
        }

        public override int GetExecutionCosts() {
            throw new NotImplementedException();
        }

        public override int GetPriority() {
            throw new NotImplementedException();
        }

        public override void Execute() {}
    }

}