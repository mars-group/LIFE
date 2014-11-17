﻿using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GOAPModelDefinition.Worldstates;

namespace GOAPModelDefinition.Actions
{

    public class ActionThinkPositiv : AbstractGoapAction
    {
        public ActionThinkPositiv()
            : base(new List<WorldstateSymbol>(),
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.Happy, true, typeof (Boolean)),
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