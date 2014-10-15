using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using GOAPModelDefinition.Worldstates;

namespace GOAPModelDefinition.Actions {
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