using System;
using System.Collections.Generic;
using GoapBetaCommon.Implementation;
using GOAPBetaModelDefinition.Worldstates;
using GoapBetaCommon.Abstract;

namespace GOAPBetaModelDefinition.Actions
{
    public class ActionStealToy : AbstractGoapAction
    {

        public ActionStealToy()
            : base(new List<WorldstateSymbol>(),
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.HasToy,true,typeof(Boolean)),
                }) { }

        public override bool ValidateContextPreconditions()
        {
            return true;
        }

        public override bool ExecuteContextEffects()
        {
            throw new NotImplementedException();
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }

        public override int GetExecutionCosts()
        {
            return 1;
        }

        public override int GetPriority()
        {
            throw new NotImplementedException();
        }
    }
}