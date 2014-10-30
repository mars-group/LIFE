using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using GOAPModelDefinition.Worldstates;

namespace GOAPModelDefinition.Actions {
    public class ActionPlay : AbstractGoapAction {
        public ActionPlay()
            : base(new List<IGoapWorldProperty> {new HasToy(true)},
                new List<IGoapWorldProperty> {
                    new IsHappy(true),
                    new HasToy(false)
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