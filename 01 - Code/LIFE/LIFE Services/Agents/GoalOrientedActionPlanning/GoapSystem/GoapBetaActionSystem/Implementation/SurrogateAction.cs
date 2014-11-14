using System.Collections.Generic;
using GoapBetaCommon.Abstract;
using GoapBetaCommon.Implementation;

namespace GoapBetaActionSystem.Implementation {

    /// <summary>
    ///     null action class
    /// </summary>
    internal class SurrogateAction : AbstractGoapAction {
        public SurrogateAction()
            : base(new List<WorldstateSymbol>(), new List<WorldstateSymbol>()) {}

        public override bool ValidateContextPreconditions() {
            return true;
        }

        public override bool ExecuteContextEffects() {
            return true;
        }

        public override void Execute() {}

        public override int GetExecutionCosts() {
            return 0;
        }

        public override int GetPriority() {
            return 0;
        }
    }

}