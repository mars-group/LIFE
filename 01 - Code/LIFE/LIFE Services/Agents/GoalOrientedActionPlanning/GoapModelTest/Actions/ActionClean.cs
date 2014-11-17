using System;
using System.Collections.Generic;
using GoapBetaCommon.Abstract;
using GoapBetaCommon.Implementation;
using GoapBetaCommon.Interfaces;
using GoapBetaCommon.Interfaces;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Actions {
    public class ActionClean : AbstractGoapAction
    {
        public ActionClean()
            : base(new List<WorldstateSymbol> { new WorldstateSymbol(WorldProperties.Happy, true, typeof(Boolean)) },
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.Happy, false, typeof (Boolean)),
                    new WorldstateSymbol(WorldProperties.HasMoney, true, typeof (Boolean)),
                }) { }

        public override bool ValidateContextPreconditions()
        {
            return true;
        }

        public override void Execute() { }

        public override bool ExecuteContextEffects()
        {
            throw new NotImplementedException();
        }

        public override int GetExecutionCosts()
        {
            return 1;
        }

        public override int GetPriority()
        {
            return 1;
        }
    }
}