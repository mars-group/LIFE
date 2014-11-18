using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;

namespace GoapActionSystem.Implementation {

    /// <summary>
    ///     null action class
    /// </summary>
    public class SurrogateAction : AbstractGoapAction {
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