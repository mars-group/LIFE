using System;
using System.Collections.Generic;
using CommonTypes.Interfaces;
using GoapActionSystem.Implementation;
using GoapCommon.Interfaces;
using GoapUser.Worldstates;

namespace GoapUser.Actions {

    public class ActionGetToy : AbstractGoapAction {

        public ActionGetToy(List<IGoapWorldstate> preconditionWorldstates, List<IGoapWorldstate> effectWorldstates)
            : base(preconditionWorldstates: preconditionWorldstates, effectWorldstates: effectWorldstates) {}

        public ActionGetToy()
            : base(new List<IGoapWorldstate> { new HasMoney(true, WorldStateEnums.HasMoney) },
                new List<IGoapWorldstate> {
                    new HasMoney(false, WorldStateEnums.HasMoney),
                    new HasToy(true, WorldStateEnums.HasToy)
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
    }
}