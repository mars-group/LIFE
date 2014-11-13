using System;
using System.Collections.Generic;
using GOAPBetaModelDefinition.Worldstates;
using GoapBetaCommon.Abstract;
using GoapBetaCommon.Interfaces;

namespace GOAPBetaModelDefinition.Actions {
    public class ActionClean : AbstractGoapAction {
        public ActionClean()
            : base(new List<IGoapWorldProperty> {new IsHappy(true)},
                new List<IGoapWorldProperty> {
                    new IsHappy(false),
                    new HasMoney(true)
                }) {}

        public override bool ValidateContextPreconditions() {
            return true;
        }

        public override void Execute() {
           
        }

        public override bool ExecuteContextEffects() {
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