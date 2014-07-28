using System;
using System.Collections.Generic;
using GoapCommon.Interfaces;

namespace GoapActionSystem.Implementation {
    internal class SurrogateAction : AbstractGoapAction {

        public SurrogateAction()
            : base(new List<IGoapWorldstate>(), new List<IGoapWorldstate>()) { }

        public override bool ValidateContextPreconditions() {
            throw new NotImplementedException();
        }

        public override bool ExecuteContextEffects() {
            return true;
        }

        public override bool Execute() {
            return true;
        }
    }
}