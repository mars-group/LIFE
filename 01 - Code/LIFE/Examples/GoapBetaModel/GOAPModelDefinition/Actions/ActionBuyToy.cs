using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GOAPModelDefinition.Worldstates;

namespace GOAPModelDefinition.Actions {
    public class ActionBuyToy : AbstractGoapAction {

        public ActionBuyToy()
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