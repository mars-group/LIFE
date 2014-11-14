using System;
using System.Collections.Generic;
using GoapBetaCommon.Implementation;
using GOAPBetaModelDefinition.Worldstates;
using GoapBetaCommon.Abstract;
using GoapBetaCommon.Interfaces;

namespace GOAPBetaModelDefinition.Actions {
    public class ActionGetToy : AbstractGoapAction {

        public ActionGetToy()
            : base(new List<WorldstateSymbol> { new WorldstateSymbol(WorldProperties.HasMoney, true, typeof(Boolean)) },
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.HasMoney,false,typeof(Boolean)),
                    new WorldstateSymbol(WorldProperties.HasToy,true,typeof(Boolean)),
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