using System;
using System.Collections.Generic;
using GoapBetaCommon.Abstract;
using GoapBetaCommon.Implementation;
using GOAPBetaModelDefinition.Worldstates;

namespace GOAPBetaModelDefinition.Actions {

    public class ActionPlay : AbstractGoapAction {
        public ActionPlay()
            : base(new List<WorldstateSymbol> {new WorldstateSymbol(WorldProperties.HasToy, true, typeof (Boolean))},
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.Happy, true, typeof (Boolean)),
                    new WorldstateSymbol(WorldProperties.HasToy, false, typeof (Boolean)),
                }) {}

        public override bool ValidateContextPreconditions() {
            return true;
        }

        public override bool ExecuteContextEffects() {
            throw new NotImplementedException();
        }

        public override void Execute() {
            throw new NotImplementedException();
        }

        public override int GetExecutionCosts() {
            return 1;
        }

        public override int GetPriority() {
            throw new NotImplementedException();
        }
    }

}