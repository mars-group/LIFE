using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;

namespace GoapActionSystem.Implementation {
    /// <summary>
    /// null action class
    /// </summary>
    internal class SurrogateAction : AbstractGoapAction {
        public SurrogateAction()
            : base(new List<IGoapWorldstate>(), new List<IGoapWorldstate>()) {}

        public override bool ValidateContextPreconditions() {
            return true;
        }

        public override bool ExecuteContextEffects() {
            return true;
        }

        public override bool Execute() {
            return true;
        }

        public override int ExecutionCosts() {
            throw new NotImplementedException();
        }

        public override int Precedence() {
            throw new NotImplementedException();
        }
    }
}