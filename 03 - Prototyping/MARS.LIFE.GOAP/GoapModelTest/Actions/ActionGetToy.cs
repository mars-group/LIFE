using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Actions {
    public class ActionGetToy : AbstractGoapAction {
        public ActionGetToy(List<IGoapWorldstate> preconditionWorldstates, List<IGoapWorldstate> effectWorldstates)
            : base(preconditionWorldstates, effectWorldstates) {}

        public ActionGetToy()
            : base(new List<IGoapWorldstate> {new HasMoney(true)},
                new List<IGoapWorldstate> {
                    new HasMoney(false),
                    new HasToy(true)
                }) {}

        public override bool ValidateContextPreconditions() {
            throw new NotImplementedException();
        }

        public override bool ExecuteContextEffects() {
            throw new NotImplementedException();
        }

        public override bool Execute() {
            throw new NotImplementedException();
        }

        public override int ExecutionCosts() {
            throw new NotImplementedException();
        }

        public override int Precedence() {
            throw new NotImplementedException();
        }
    }
}