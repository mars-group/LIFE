using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Actions {

    public class ActionClean : AbstractGoapAction {

        public ActionClean(List<IGoapWorldstate> preconditionWorldstates, List<IGoapWorldstate> effectWorldstates)
            : base(preconditionWorldstates, effectWorldstates) {}

        public ActionClean()
            : base(new List<IGoapWorldstate> {new Happy(true)}, new List<IGoapWorldstate> {
                new Happy(false),
                new HasMoney(true)
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
            return 1;
        }
    }
}
