using System;
using System.Collections.Generic;
using CommonTypes.Interfaces;
using GoapActionSystem.Implementation;
using GoapCommon.Interfaces;
using GoapUser.Worldstates;

namespace GoapUser.Actions {

    internal class ActionClean : AbstractGoapAction {

        public ActionClean(List<IGoapWorldstate> preconditionWorldstates, List<IGoapWorldstate> effectWorldstates)
            : base(preconditionWorldstates, effectWorldstates) {}

        public ActionClean()
            : base(new List<IGoapWorldstate> {new Happy(true, WorldStateEnums.Happy)}, new List<IGoapWorldstate> {
                new Happy(false, WorldStateEnums.Happy),
                new HasMoney(true, WorldStateEnums.HasMoney)
            }) {}

        public override bool ValidateContextPreconditions() {
            return true;
        }

        public override bool ExecuteContextEffects() {
            throw new NotImplementedException();
        }

        public override bool Execute() {
            throw new NotImplementedException();
        }
    }
}
