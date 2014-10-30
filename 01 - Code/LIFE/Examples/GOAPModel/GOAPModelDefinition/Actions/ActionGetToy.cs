using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using GOAPModelDefinition.Worldstates;

namespace GOAPModelDefinition.Actions {
    public class ActionGetToy : AbstractGoapAction {
        public ActionGetToy()
            : base(new List<IGoapWorldProperty> {new HasMoney(true)},
                new List<IGoapWorldProperty> {
                    new HasMoney(false),
                    new HasToy(true)
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