using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition.Actions {

    public class AimlessWandering : AbstractGoapAction {

        public AimlessWandering
            (List<WorldstateSymbol> preconditionWorldstates, List<WorldstateSymbol> effectWorldstates) :
                base(
            new List<WorldstateSymbol> {new WorldstateSymbol(Properties.HasTarget,false,typeof(Boolean))},
            new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsInExitArea,true,typeof(Boolean))}) {}

        public override bool ValidateContextPreconditions() {
            return true;
        }

        public override bool ExecuteContextEffects() {
            throw new NotImplementedException();
        }

        public override int GetExecutionCosts() {
            return 1;
        }

        public override void Execute() {}
    }

}